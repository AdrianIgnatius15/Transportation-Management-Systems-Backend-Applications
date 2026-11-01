package com.example.tms.document_service_rest_api.controllers

import com.example.tms.document_service_rest_api.repositories.services.OrderRepositoryService
import org.springframework.web.bind.annotation.PathVariable
import org.springframework.web.bind.annotation.PostMapping
import org.springframework.web.bind.annotation.RequestMapping
import org.springframework.web.bind.annotation.RestController

@RestController
@RequestMapping("api/document")
class DocumentController(private val orderRepository: OrderRepositoryService) {
    @PostMapping("upload/{orderId}")
    fun uploadShipmentDocument(@PathVariable("orderId") orderId: String) {
        
    }
}