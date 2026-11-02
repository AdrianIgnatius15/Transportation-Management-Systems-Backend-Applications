package com.example.tms.document_service_rest_api.controllers

import com.example.tms.document_service_rest_api.models.Order
import com.example.tms.document_service_rest_api.models.dtos.PageResponse
import com.example.tms.document_service_rest_api.repositories.services.OrderRepositoryService
import io.swagger.v3.oas.annotations.Operation
import io.swagger.v3.oas.annotations.tags.Tag
import org.springframework.data.domain.PageRequest
import org.springframework.data.domain.Sort
import org.springframework.web.bind.annotation.GetMapping
import org.springframework.web.bind.annotation.RequestMapping
import org.springframework.web.bind.annotation.RequestParam
import org.springframework.web.bind.annotation.RestController
import java.util.UUID

@RestController
@RequestMapping("api/order")
@Tag(name = "Order Management", description = "APIs for managing orders for shipment")
class OrderController(private val orderRepositoryService: OrderRepositoryService) {

    @GetMapping("/paginate/orders")
    @Operation(
        summary = "Retrieve all orders with pagination",
        description = "Retrieves all orders with pagination but in an ascending order based on column name criteria"
    )
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

    @GetMapping("/paginate-cursor/orders")
    @Operation(
        summary = "Retrieves all orders through cursor pagination",
        description = "Retrieves all orders using cursor pagination but in an ascending order based on column name"
    )
    fun getAllOrdersCursorPaginated(
        @RequestParam(required = false) cursor: UUID?,
        @RequestParam(defaultValue = "0") size: Int,
        @RequestParam(defaultValue = "createdAt") sortBy: String,
        @RequestParam(defaultValue = "ASC") direction: Sort.Direction
    ): PageResponse<Order> {
        val safeSize = size.coerceAtMost(100)
        val pageable = PageRequest.of(0, safeSize, Sort.by(direction, sortBy))

        val orderSliced = orderRepositoryService.getAllOrdersCursorPagination(cursor, pageable)

        return PageResponse(
            orderSliced.content,
            orderSliced.number,
            orderSliced.size,
            orderSliced.hasNext()
        )
    }
}