package com.example.tms.document_service_rest_api.models

import java.util.Date
import org.springframework.data.annotation.Id
import org.springframework.data.relational.core.mapping.Table

@Table("Orders")
data class Order(
    @Id
    val id: String? = null,
    val clientId: String,
    val orderNumber: String,
    val priority: String,
    val createdAt: Date,
    val updatedAt: Date,
)
