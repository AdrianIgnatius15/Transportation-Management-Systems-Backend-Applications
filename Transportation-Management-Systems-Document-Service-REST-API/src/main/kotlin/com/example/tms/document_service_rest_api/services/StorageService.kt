package com.example.tms.document_service_rest_api.services

import io.github.oshai.kotlinlogging.KotlinLogging
import org.springframework.beans.factory.annotation.Value
import org.springframework.core.io.ByteArrayResource
import org.springframework.http.MediaType
import org.springframework.stereotype.Service
import org.springframework.web.client.RestClient
import software.amazon.awssdk.core.sync.RequestBody
import software.amazon.awssdk.services.s3.S3Client
import software.amazon.awssdk.services.s3.model.DeleteObjectRequest
import software.amazon.awssdk.services.s3.model.DeleteObjectResponse
import software.amazon.awssdk.services.s3.model.GetObjectRequest
import software.amazon.awssdk.services.s3.model.HeadObjectRequest
import software.amazon.awssdk.services.s3.model.NoSuchKeyException
import software.amazon.awssdk.services.s3.model.PutObjectRequest
import software.amazon.awssdk.services.s3.model.S3Exception
import software.amazon.awssdk.services.s3.presigner.S3Presigner
import software.amazon.awssdk.services.s3.presigner.model.GetObjectPresignRequest
import java.time.Duration

@Service
class StorageService(
    private val s3Client: S3Client,
    private val s3Presigner: S3Presigner,
    @Value($$"${seaweedfs.bucket-name}") private val bucketName: String,
    @Value($$"${seaweedfs.internal-endpoint}") private val internalEndpoint: String,
    @Value($$"${seaweedfs.public-endpoint}") private val publicEndpoint: String,
    @Value($$"${seaweedfs.filer.url}")
    private val fileUrl: String
) {

    companion object {
        private val TAG = StorageService::class.java.name
        private val logger = KotlinLogging.logger {}
    }

    fun checkIfShipmentDocumentExists(orderId: String, fileKey: String): Boolean {
        val documentHeadObject = HeadObjectRequest.builder()
            .bucket(bucketName)
            .key("$orderId/$fileKey")
            .build()

        return try {
            s3Client.headObject(documentHeadObject)
            true
        } catch (e: NoSuchKeyException) {
            false
        } catch (e: Exception) {
            logger.error { "$TAG Error when checking for the document in SeaweedFS due to ${e.message}" }
            false
        }
    }

    fun generatePresignedDownloadUrl(fileKey: String, durationMinutes: Long = 10): String {
        val getObjectRequest = GetObjectRequest.builder()
            .bucket("tms-documents")
            .key(fileKey)
            .build()

        val presignedRequest = GetObjectPresignRequest.builder()
            .signatureDuration(Duration.ofMinutes(durationMinutes))
            .getObjectRequest(getObjectRequest)
            .build()

        val presignedUrl = s3Presigner.presignGetObject(presignedRequest).url().toString()
        return presignedUrl.replace(internalEndpoint, publicEndpoint)
    }

    fun uploadShipmentDocuments(
        fileKey: String,
        orderId: String,
        fileBytes: ByteArray,
        contentType: String = "application/octet-stream"
    ): Boolean {
        val putObjectRequest = PutObjectRequest.builder()
            .bucket(bucketName)
            .key("$orderId/$fileKey")
            .contentType(contentType)
            .build()

        return try {
            s3Client.putObject(putObjectRequest, RequestBody.fromBytes(fileBytes))
            true
        } catch (e: S3Exception) {
            logger.error { "$TAG Error occured on SeaweedFS due to ${e.message}" }
            false
        } catch (e: Exception) {
            logger.error { "$TAG Error when checking for the document in SeaweedFS due to ${e.message}" }
            false
        }
    }

    fun deleteShipmentDocument(fileKey: String, orderId: String): DeleteObjectResponse {
        val deleteObjectRequest = DeleteObjectRequest.builder()
            .bucket(bucketName)
            .key("$orderId/$fileKey")
            .build()

        return s3Client.deleteObject(deleteObjectRequest)
    }
}
