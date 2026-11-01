package com.example.tms.document_service_rest_api.configurations

import org.springframework.beans.factory.annotation.Value
import org.springframework.context.annotation.Bean
import org.springframework.context.annotation.Configuration
import software.amazon.awssdk.auth.credentials.AwsBasicCredentials
import software.amazon.awssdk.auth.credentials.StaticCredentialsProvider
import software.amazon.awssdk.regions.Region
import software.amazon.awssdk.services.s3.S3Client
import software.amazon.awssdk.services.s3.S3Configuration
import software.amazon.awssdk.services.s3.presigner.S3Presigner
import java.net.URI

@Configuration
class SeaweedFSConfig(
    @Value($$"${seaweedfs.access-key}") accessKey: String,
    @Value($$"${seaweedfs.secret-key}") secretKey: String,
    @Value($$"${seaweedfs.region}") private val region: String,
    @Value($$"${seaweedfs.internal-endpoint}") private val internalEndpoint: String
) {
    private val credentials = AwsBasicCredentials.create(accessKey, secretKey)
    private val s3Configuration = S3Configuration.builder().pathStyleAccessEnabled(true).build()

    @Bean
    fun s3Presigner(): S3Presigner {

        val s3Configuration = S3Configuration.builder()
            .pathStyleAccessEnabled(true)
            .build()

        return S3Presigner.builder()
            .region(Region.of(region))
            .credentialsProvider(StaticCredentialsProvider.create(credentials))
            .endpointOverride(URI.create(internalEndpoint))
            .serviceConfiguration(s3Configuration)
            .build()
    }

    @Bean
    fun s3Client(): S3Client {
        return S3Client.builder()
            .region(Region.of(region))
            .credentialsProvider(StaticCredentialsProvider.create(credentials))
            .endpointOverride(URI.create(internalEndpoint))
            .serviceConfiguration(s3Configuration)
            .build()
    }
}