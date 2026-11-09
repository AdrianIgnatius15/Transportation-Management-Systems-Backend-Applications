package com.example.tms.document_service_rest_api.models.sealedmodels

import com.example.tms.document_service_rest_api.models.enums.DocumentType
import java.util.UUID

sealed class TMSDocumentEvent(
    val eventId: String = UUID.randomUUID().toString(),
    val eventType: String,
    val timestamp: Long = System.currentTimeMillis()
)

data class DocumentDeletedEvent(
    val orderId: String,
    val folderPath: String
) : TMSDocumentEvent(eventType = "DOCUMENT_DELETED")

data class DocumentCreatedEvent(
    val orderId: String,
    val fileName: String,
    val documentType: DocumentType,
    val fileSize: Long,
) : TMSDocumentEvent(eventType = "DOCUMENT_CREATED")