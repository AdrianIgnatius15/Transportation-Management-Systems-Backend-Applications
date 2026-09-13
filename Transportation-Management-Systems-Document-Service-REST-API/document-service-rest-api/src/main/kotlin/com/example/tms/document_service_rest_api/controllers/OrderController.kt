package com.example.tms.document_service_rest_api.controllers

import com.example.tms.document_service_rest_api.models.Order
import com.example.tms.document_service_rest_api.models.dtos.PageResponse
import com.example.tms.document_service_rest_api.repositories.services.OrderRepositoryService
import org.springframework.data.domain.PageRequest
import org.springframework.data.domain.Sort
import org.springframework.web.bind.annotation.GetMapping
import org.springframework.web.bind.annotation.RequestMapping
import org.springframework.web.bind.annotation.RequestParam
import org.springframework.web.bind.annotation.RestController

@RestController
@RequestMapping("/order")
class OrderController(private val orderRepositoryService: OrderRepositoryService) {

    @GetMapping("/paginate/orders")
    fun getAllOrdersPaginated(
        @RequestParam(defaultValue = "0") page: Int,
        @RequestParam(defaultValue = "20") size: Int,
        @RequestParam(defaultValue = "createdAt") sortBy: String,
        @RequestParam(defaultValue = "ASC") direction: Sort.Direction
    ): PageResponse<Order> {
        val safeSize = size.coerceAtMost(100)
        val pageable = PageRequest.of(page, safeSize, Sort.by(direction, sortBy))

        val orderSliced = orderRepositoryService.getAllOrdersPaginated(pageable)

        return PageResponse(
            orderSliced.content,
            orderSliced.number,
            orderSliced.size,
            orderSliced.hasNext()
        )
    }
}