package com.example.tms.document_service_rest_api.models.messages

data class OrderCreatedEvent(
    val id: String,
    val clientId: String,
    val orderNumber: String,
    val orderStatus: String
)