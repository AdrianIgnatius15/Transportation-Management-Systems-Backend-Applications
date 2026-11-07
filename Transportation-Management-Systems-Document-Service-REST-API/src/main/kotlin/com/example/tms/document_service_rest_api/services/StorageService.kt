package com.example.tms.document_service_rest_api.services

import com.example.tms.document_service_rest_api.models.enums.DocumentType
import com.example.tms.document_service_rest_api.repositories.services.DocumentRepositoryService
import io.github.oshai.kotlinlogging.KotlinLogging
import org.springframework.beans.factory.annotation.Value
import org.springframework.core.io.ByteArrayResource
import org.springframework.http.MediaType
import org.springframework.stereotype.Service
import org.springframework.web.client.RestClient
import org.springframework.web.multipart.MultipartFile
import software.amazon.awssdk.core.sync.RequestBody
import software.amazon.awssdk.services.s3.S3Client
import software.amazon.awssdk.services.s3.model.Delete
import software.amazon.awssdk.services.s3.model.DeleteBucketRequest
import software.amazon.awssdk.services.s3.model.DeleteObjectRequest
import software.amazon.awssdk.services.s3.model.DeleteObjectResponse
import software.amazon.awssdk.services.s3.model.DeleteObjectsRequest
import software.amazon.awssdk.services.s3.model.GetObjectRequest
import software.amazon.awssdk.services.s3.model.HeadObjectRequest
import software.amazon.awssdk.services.s3.model.ListObjectsV2Request
import software.amazon.awssdk.services.s3.model.NoSuchKeyException
import software.amazon.awssdk.services.s3.model.ObjectIdentifier
import software.amazon.awssdk.services.s3.model.PutObjectRequest
import software.amazon.awssdk.services.s3.model.S3Exception
import software.amazon.awssdk.services.s3.presigner.S3Presigner
import software.amazon.awssdk.services.s3.presigner.model.GetObjectPresignRequest
import java.time.Duration
import java.util.UUID

@Service
class StorageService(
    private val s3Client: S3Client,
    private val s3Presigner: S3Presigner,
    @Value($$"${seaweedfs.bucket-name}") private val bucketName: String,
    @Value($$"${seaweedfs.internal-endpoint}") private val internalEndpoint: String,
    @Value($$"${seaweedfs.public-endpoint}") private val publicEndpoint: String,
    @Value($$"${seaweedfs.filer.url}")
    private val fileUrl: String,
    private val documentRepositoryService: DocumentRepositoryService
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
            .bucket(bucketName)
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
            val uploadedStatus = s3Client.putObject(putObjectRequest, RequestBody.fromBytes(fileBytes))
            if (!uploadedStatus.sdkHttpResponse().isSuccessful) {
                logger.error { "$TAG Upload failed for $orderId/$fileKey" }
                return false
            }
            documentRepositoryService.createDocument(UUID.fromString(orderId), DocumentType.Other, fileKey)
            true
        } catch (e: S3Exception) {
            logger.error { "$TAG Error occured on SeaweedFS due to ${e.message}" }
            false
        } catch (e: Exception) {
            logger.error { "$TAG Error when checking for the document in SeaweedFS due to ${e.message}" }
            false
        }
    }

    fun uploadMultipartShipmentDocuments(orderId: String, files: List<MultipartFile>): Boolean {
        var allFileUploadedStatus = true
        for (file in files) {
            val fileKey = file.originalFilename ?: "unnamed_${System.currentTimeMillis()}"
            try {
                val putObjectRequest = PutObjectRequest.builder()
                    .bucket(bucketName)
                    .key("$orderId/$fileKey")
                    .contentType("application/octet-stream")
                    .build()

                val responseUploadStatus = s3Client.putObject(putObjectRequest, RequestBody.fromBytes(file.bytes))

                if (responseUploadStatus.sdkHttpResponse().isSuccessful) {
                    logger.debug { "The file is uploaded succesfully" }
                    documentRepositoryService.createDocument(UUID.fromString(orderId), DocumentType.Other, fileKey)
                } else {
                    logger.debug {
                        "The file didn't upload successfully due to ${
                            responseUploadStatus.sdkHttpResponse().statusText()
                        }"
                    }
                }
            } catch (e: S3Exception) {
                logger.error { "$TAG Error occured on SeaweedFS due to ${e.message}" }
                allFileUploadedStatus = false
            } catch (e: Exception) {
                logger.error { "$TAG Error when checking for the document in SeaweedFS due to ${e.message}" }
                allFileUploadedStatus = false
            }
        }

        return allFileUploadedStatus
    }

    fun deleteShipmentDocument(fileKey: String, orderId: String): Boolean {
        return try {
            val deleteObjectRequest = DeleteObjectRequest.builder()
                .bucket(bucketName)
                .key("$orderId/$fileKey")
                .build()

            s3Client.deleteObject(deleteObjectRequest)
            documentRepositoryService.deleteDocumentsBasedOnOrderIdAndFilename(UUID.fromString(orderId), fileKey)
            true
        } catch (e: S3Exception) {
            logger.error { "Error while deleting the file due to: ${e.message}" }
            false
        }
    }

    fun deleteAllShipmentDocumentsViaBucket(orderId: String): Boolean {
        return try {
            val listRequest = ListObjectsV2Request.builder()
                .bucket(bucketName)
                .prefix("$orderId/")
                .build()

            val listResponse = s3Client.listObjectsV2(listRequest)

            val objectsToDelete = listResponse.contents().map {
                ObjectIdentifier.builder().key(it.key()).build()
            }
            val deleteObjectBucketRequest = DeleteObjectsRequest.builder()
                .bucket(bucketName)
                .delete(
                    Delete.builder()
                        .objects(objectsToDelete)
                        .build()
                )
                .build()

            s3Client.deleteObjects(deleteObjectBucketRequest)
            documentRepositoryService.deleteDocumentsBasedOnOrderId(UUID.fromString(orderId))
            true
        } catch (e: S3Exception) {
            logger.error { "Error while deleting the file due to: ${e.message}" }
            false
        }
    }
}
