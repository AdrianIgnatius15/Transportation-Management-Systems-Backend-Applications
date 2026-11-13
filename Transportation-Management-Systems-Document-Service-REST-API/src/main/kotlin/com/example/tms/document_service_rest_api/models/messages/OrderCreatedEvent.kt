package com.example.tms.document_service_rest_api.models.messages

data class OrderCreatedEvent(
    val orderId: String,
    val clientId: String,
    val orderNumber: String
)