package com.example.tms.document_service_rest_api.controllers

import com.example.tms.document_service_rest_api.services.StorageService
import io.swagger.v3.oas.annotations.Operation
import io.swagger.v3.oas.annotations.tags.Tag
import org.springframework.http.HttpStatus
import org.springframework.http.HttpStatusCode
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

        return if (uploadedFileStatus) {
            ResponseEntity.status(HttpStatus.CREATED).body(
                mapOf("message" to "Document uploaded successfully", "path" to "$orderId/$fileKey")
            )
        } else {
            ResponseEntity.status(HttpStatus.CREATED).body(
                mapOf("message" to "Document did not uploaded successfully")
            )
        }
    }

    @PostMapping("/upload-shipment-files/{orderId}")
    @Operation(
        summary = "Upload a list of shipment document by shipment order ID",
        description = "Uploads a list of shipment documents into SeaweedFS and storing it on a folder with the folder name being the shipment order ID"
    )
    fun uploadShipmentDocuments(
        @PathVariable("orderId") orderId: String,
        @RequestParam("files") files: List<MultipartFile>
    ): ResponseEntity<Map<String, String>> {
        if (files.isEmpty()) {
            return ResponseEntity.badRequest().body(
                mapOf("message" to "Shipment documents have not been passed succesfully. Please check again!")
            )
        } else {
            val uploadedShipmentDocumentsStatus = storageService.uploadMultipartShipmentDocuments(orderId, files)

            return if (uploadedShipmentDocumentsStatus) {
                ResponseEntity.status(HttpStatus.CREATED).body(
                    mapOf("message" to "All files uploaded successfully")
                )
            } else {
                ResponseEntity.status(HttpStatus.BAD_GATEWAY).body(
                    mapOf("message" to "One or more files could not be uploaded")
                )
            }
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
    fun deleteADocumentBasedOnOrderIdAndFilename(
        @PathVariable("orderId") orderId: String,
        @PathVariable("fileName") fileKey: String
    ): ResponseEntity<Map<String, String>> {
        val deletedDetails = storageService.deleteShipmentDocument(
            fileKey = fileKey,
            orderId = orderId
        )

        if (deletedDetails) {
            return ResponseEntity.status(HttpStatus.OK).body(
                mapOf("message" to "Document deleted succesfully")
            )
        }

        return ResponseEntity.status(HttpStatus.OK).body(
            mapOf("message" to "Document did not delete succesfully")
        )
    }

    @DeleteMapping("/delete-files/{orderId}")
    fun deleteAllDocumentsBasedOnOrderId(
        @PathVariable("orderId") orderId: String
    ): ResponseEntity<Map<String, String>> {
        val deletedStatus = storageService.deleteAllShipmentDocumentsViaBucket(orderId)

        return if (deletedStatus) {
            ResponseEntity.status(HttpStatus.OK).body(
                mapOf("message" to "The files for the $orderId together with the folder has been deleted successfully")
            )
        } else {
            ResponseEntity.status(HttpStatus.HTTP_VERSION_NOT_SUPPORTED).body(
                mapOf("message" to "Document did not delete succesfully")
            )
        }
    }
}