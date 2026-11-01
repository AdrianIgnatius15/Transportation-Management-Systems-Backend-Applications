package com.example.tms.document_service_rest_api.repositories.interfaces

import com.example.tms.document_service_rest_api.models.Document
import org.springframework.data.jpa.repository.Query
import org.springframework.data.repository.CrudRepository
import org.springframework.data.repository.query.Param
import java.util.UUID

interface DocumentRepository : CrudRepository<Document, UUID> {
    @Query(
        """
        SELECT documents FROM Documents documents 
        WHERE documents.order.id = :orderId
        ORDER BY documents.id ASC
    """
    )
    fun getAllDocumentsByOrderId(@Param("orderId") orderId: UUID): List<Document>
}