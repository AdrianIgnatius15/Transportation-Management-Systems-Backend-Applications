package com.example.tms.document_service_rest_api.models

import jakarta.persistence.Entity
import jakarta.persistence.Id
import jakarta.persistence.Table
import jakarta.persistence.Column
import java.util.Date
import java.util.UUID

@Entity(name = "Orders")
@Table(name = "Orders", schema = "tms")
data class Order(
    @Id
    @Column(name = "Id")
    val id: UUID? = null,
    @Column(name = "ClientId")
    val clientId: UUID,
    @Column(name = "OrderNumber")
    val orderNumber: String,
    @Column(name = "Status")
    val status: Int,
    @Column(name = "Priority")
    val priority: String,
    @Column(name = "ShipmentAddressId")
    val shipmentAddressId: UUID,
    @Column(name = "DeliveryAddressId")
    val deliveryAddressId: UUID,
    @Column(name = "CreatedAt")
    val createdAt: Date,
    @Column(name = "UpdatedAt")
    val updatedAt: Date,
)
