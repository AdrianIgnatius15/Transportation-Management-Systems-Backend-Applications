package com.example.tms.document_service_rest_api.repositories.services

import com.example.tms.document_service_rest_api.models.Document
import com.example.tms.document_service_rest_api.models.enums.DocumentType
import com.example.tms.document_service_rest_api.repositories.interfaces.DocumentRepository
import org.springframework.stereotype.Service
import java.time.LocalDate
import java.util.Date
import java.util.UUID

@Service
class DocumentRepositoryService(private val documentRepository: DocumentRepository) {
    fun createDocument(orderId: UUID, documentType: DocumentType, fileName: String) {
        val document = Document(
            orderId = orderId,
            documentType = documentType,
            fileName = fileName,
            createdAt = Date()
        )
        documentRepository.save(document)
    }

    fun getDocumentById(id: UUID): Document? {
        val document = documentRepository.findById(id)

        return if (document.isEmpty) {
            null
        } else {
            document.get()
        }
    }

    fun deleteDocument(id: UUID) {
        documentRepository.deleteById(id)
    }
}