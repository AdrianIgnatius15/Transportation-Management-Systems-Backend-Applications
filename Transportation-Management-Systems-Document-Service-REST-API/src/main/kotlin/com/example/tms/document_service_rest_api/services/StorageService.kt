package com.example.tms.document_service_rest_api.services

import org.springframework.beans.factory.annotation.Value
import org.springframework.core.io.ByteArrayResource
import org.springframework.http.MediaType
import org.springframework.web.client.RestClient
import software.amazon.awssdk.services.s3.S3Client
import software.amazon.awssdk.services.s3.model.GetObjectRequest
import software.amazon.awssdk.services.s3.model.PutObjectRequest
import software.amazon.awssdk.services.s3.presigner.S3Presigner
import software.amazon.awssdk.services.s3.presigner.model.GetObjectPresignRequest
import java.time.Duration

class StorageService(
    private val s3Client: S3Client,
    private val s3Presigner: S3Presigner,
    @Value($$"${seaweedfs.bucket-name}") private val bucketName: String,
    @Value($$"${seaweedfs.internal-endpoint}") private val internalEndpoint: String,
    @Value($$"${seaweedfs.public-endpoint}") private val publicEndpoint: String,
    @Value($$"${seaweedfs.filer.url}")
    private val fileUrl: String
) {
    
    fun retrieveShipmentDocuments() {}
    
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
        return presignedUrl.replace(internalEndpoint, fileUrl)
    }
    
    fun uploadShipmentDocuments(fileKey: String, orderId: String, fileBytes: ByteArray, contentType: String = "application/octet-stream") {
        val putObjectRequest = PutObjectRequest.builder()
            .bucket(bucketName)
            .key("$orderId/$fileKey")
            .contentType
    }
}
