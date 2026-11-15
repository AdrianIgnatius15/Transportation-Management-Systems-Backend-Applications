package com.example.tms.document_service_rest_api.models.messages

import com.fasterxml.jackson.annotation.JsonAlias

data class OrderCreatedEvent(
    @JsonAlias("Id")
    val id: String,
    @JsonAlias("ClientId")
    val clientId: String,
    @JsonAlias("OrderNumber")
    val orderNumber: String,
    @JsonAlias("OrderStatus")
    val orderStatus: String
)