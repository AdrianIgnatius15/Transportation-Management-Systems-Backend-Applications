package com.example.tms.document_service_rest_api.repositories.services

import com.example.tms.document_service_rest_api.models.Order
import com.example.tms.document_service_rest_api.repositories.interfaces.OrderRepository
import org.springframework.data.domain.Pageable
import org.springframework.stereotype.Service

@Service
class OrderRepositoryService(private val orderRepository: OrderRepository) {
    fun getAllOrders() = orderRepository.findAll().toList<Order>()

    fun getAllOrdersPaginated(pageable: Pageable) = orderRepository.getAllOrdersPaginated(pageable)

    fun save(order: Order) = orderRepository.save(order)
}