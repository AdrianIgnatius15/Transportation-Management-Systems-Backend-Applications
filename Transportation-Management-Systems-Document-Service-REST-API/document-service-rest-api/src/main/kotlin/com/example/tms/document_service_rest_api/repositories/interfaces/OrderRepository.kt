package com.example.tms.document_service_rest_api.repositories.interfaces

import com.example.tms.document_service_rest_api.models.Order
import org.springframework.data.domain.Pageable
import org.springframework.data.domain.Slice
import org.springframework.data.repository.CrudRepository

interface OrderRepository : CrudRepository<Order, String> {
    fun getAllOrdersPaginated(pageable: Pageable): Slice<Order>
}