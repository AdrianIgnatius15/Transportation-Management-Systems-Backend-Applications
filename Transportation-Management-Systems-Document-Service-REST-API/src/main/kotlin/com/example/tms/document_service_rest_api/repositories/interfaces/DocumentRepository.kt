package com.example.tms.document_service_rest_api.repositories.interfaces

import com.example.tms.document_service_rest_api.models.Document
import org.springframework.data.jpa.repository.Modifying
import org.springframework.data.jpa.repository.Query
import org.springframework.data.repository.CrudRepository
import org.springframework.data.repository.query.Param
import org.springframework.transaction.annotation.Transactional
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

    @Modifying
    @Transactional
    @Query("DELETE FROM Documents documents WHERE documents.order.id = :orderId")
    fun deleteDocumentsBasedOnOrderId(@Param("orderId") orderId: UUID): Int

    @Modifying
    @Transactional
    @Query(
        """
        DELETE FROM Documents documents WHERE documents.order.id = :orderId AND
        documents.fileName = :fileName
    """
    )
    fun deleteDocumentsBasedOnOrderIdAndFilename(
        @Param("orderId") orderId: UUID,
        @Param("fileName") fileName: String
    ): Int
}