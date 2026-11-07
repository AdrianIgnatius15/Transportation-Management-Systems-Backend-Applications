package com.example.tms.document_service_rest_api.configurations

import org.junit.jupiter.api.Assertions.assertEquals
import org.junit.jupiter.api.Test
import software.amazon.awssdk.services.s3.model.GetObjectRequest
import software.amazon.awssdk.services.s3.presigner.model.GetObjectPresignRequest
import java.time.Duration

class SeaweedFSConfigTest {

    @Test
    fun `presigned URLs use the public endpoint`() {
        val presigner = SeaweedFSConfig(
            accessKey = "admin",
            secretKey = "password123",
            region = "us-east-1",
            internalEndpoint = "http://seaweedfs:8333",
            publicEndpoint = "http://localhost:8333"
        ).s3Presigner()

        val url = try {
            presigner.presignGetObject(
                GetObjectPresignRequest.builder()
                    .signatureDuration(Duration.ofMinutes(10))
                    .getObjectRequest(
                        GetObjectRequest.builder()
                            .bucket("tms-documents")
                            .key("order-id/document.pdf")
                            .build()
                    )
                    .build()
            ).url()
        } finally {
            presigner.close()
        }

        assertEquals("localhost", url.host)
        assertEquals(8333, url.port)
    }
}
