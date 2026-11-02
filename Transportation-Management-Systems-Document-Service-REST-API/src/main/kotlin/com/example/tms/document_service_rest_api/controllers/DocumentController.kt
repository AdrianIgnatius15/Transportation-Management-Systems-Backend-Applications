package com.example.tms.document_service_rest_api.controllers

import com.example.tms.document_service_rest_api.models.Document
import com.example.tms.document_service_rest_api.models.enums.DocumentType
import com.example.tms.document_service_rest_api.repositories.services.DocumentRepositoryService
import com.example.tms.document_service_rest_api.repositories.services.OrderRepositoryService
import com.example.tms.document_service_rest_api.services.StorageService
import io.swagger.v3.oas.annotations.Operation
import io.swagger.v3.oas.annotations.tags.Tag
import org.springframework.http.HttpStatus
import org.springframework.http.ResponseEntity
import org.springframework.web.bind.annotation.DeleteMapping
import org.springframework.web.bind.annotation.GetMapping
import org.springframework.web.bind.annotation.PathVariable
import org.springframework.web.bind.annotation.PostMapping
import org.springframework.web.bind.annotation.RequestMapping
import org.springframework.web.bind.annotation.RequestParam
import org.springframework.web.bind.annotation.RestController
import org.springframework.web.multipart.MultipartFile
import software.amazon.awssdk.core.Response
import java.util.UUID

@RestController
@RequestMapping("api/document")
@Tag(name = "Document Management", description = "APIs for managing user records")
class DocumentController(
    private val orderRepository: OrderRepositoryService,
    private val documentRepository: DocumentRepositoryService,
    private val storageService: StorageService
) {
    @PostMapping("upload/{orderId}")
    @Operation(
        summary = "Upload a shipment document by shipment order ID",
        description = "Uploads a document into SeaweedFS and storing it on a folder with the folder name being the shipment order ID"
    )
    fun uploadShipmentDocument(
        @PathVariable("orderId") orderId: String,
        @RequestParam("file") file: MultipartFile
    ): ResponseEntity<Map<String, String>> {
        val fileKey = file.originalFilename ?: "unnamed_${orderId}_${System.currentTimeMillis()}"
        val contentType = file.contentType ?: "application/octet-stream"

        val uploadedFileStatus = storageService.uploadShipmentDocuments(
            fileKey = fileKey,
            orderId = orderId,
            fileBytes = file.bytes,
            contentType = contentType
        )

        if (uploadedFileStatus) {
            documentRepository.createDocument(
                UUID.fromString(orderId),
                DocumentType.Waybill,
                fileKey
            )

            return ResponseEntity.status(HttpStatus.CREATED).body(
                mapOf("message" to "Document uploaded successfully", "path" to "$orderId/$fileKey")
            )
        } else {
            return ResponseEntity.status(HttpStatus.CREATED).body(
                mapOf("message" to "Document did not uploaded successfully")
            )
        }
    }

    @GetMapping("/download-url")
    @Operation(
        summary = "Gets the presigned URL of a shipment document",
        description = "Retrieves the shipment document with a presigned URL to secure document access"
    )
    fun getPresignedDownloadUrl(
        @RequestParam("fileKey") fileKey: String,
        @RequestParam("orderId") orderId: String,
        @RequestParam(value = "expiresIn", defaultValue = "10") expiresIn: Long
    ): ResponseEntity<Map<String, String>> {
        val documentExistsFlag = storageService.checkIfShipmentDocumentExists(orderId, fileKey)

        if (documentExistsFlag) {
            val url = storageService.generatePresignedDownloadUrl("$orderId/$fileKey", expiresIn)
            return ResponseEntity.ok(mapOf("url" to url))
        } else {
            return ResponseEntity.status(HttpStatus.NOT_FOUND).body(
                mapOf("message" to "The document searched does not exists")
            )
        }
    }

    @DeleteMapping("/delete-file/{orderId}/{fileName}")
    @Operation(
        summary = "Deletes a shipment document",
        description = "Deletes a shipment document based on order ID as the folder's name where the document is stored"
    )
    fun deleteMapping(
        @PathVariable("orderId") orderId: String,
        @PathVariable("fileName") fileKey: String
    ): ResponseEntity<Map<String, String>> {
        storageService.deleteShipmentDocument(
            fileKey = fileKey,
            orderId = orderId
        )

        return ResponseEntity.status(HttpStatus.OK).body(
            mapOf("message" to "Document deleted succesfully")
        )
    }
}