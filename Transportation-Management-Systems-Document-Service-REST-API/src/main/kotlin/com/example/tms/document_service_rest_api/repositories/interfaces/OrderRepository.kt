package com.example.tms.document_service_rest_api.repositories.interfaces

import com.example.tms.document_service_rest_api.models.Order
import org.springframework.data.domain.Pageable
import org.springframework.data.domain.Slice
import org.springframework.data.jpa.repository.Query
import org.springframework.data.repository.CrudRepository
import org.springframework.data.repository.query.Param
import java.util.UUID

interface OrderRepository : CrudRepository<Order, UUID> {
    @Query("SELECT o FROM Orders o")
    fun getAllOrdersPaginated(pageable: Pageable): Slice<Order>
    
    @Query("""
        SELECT o FROM Orders o
        WHERE (:cursor IS NULL OR o.id > :cursor)
        ORDER BY o.id ASC
    """)
    fun getAllOrdersCursorPagination(@Param("cursor") cursor: UUID?, pageable: Pageable): Slice<Order>
}