package com.example.tms.document_service_rest_api.models

import com.example.tms.document_service_rest_api.models.enums.DocumentType
import com.fasterxml.jackson.annotation.JsonIgnore
import jakarta.persistence.Column
import jakarta.persistence.Entity
import jakarta.persistence.FetchType
import jakarta.persistence.GeneratedValue
import jakarta.persistence.GenerationType
import jakarta.persistence.Id
import jakarta.persistence.JoinColumn
import jakarta.persistence.ManyToOne
import jakarta.persistence.Table
import java.util.Date
import java.util.UUID

@Entity(name = "Documents")
@Table(name = "Documents", schema = "tms")
data class Document(
    @Id
    @GeneratedValue(strategy = GenerationType.UUID)
    @Column(name = "Id")
    val id: UUID? = null,

    @get:JsonIgnore
    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "OrderId", nullable = false)
    val order: Order,

    @Column(name = "Type")
    val documentType: DocumentType,

    @Column(name = "FileName")
    val fileName: String,

    @Column(name = "CreatedAt")
    val createdAt: Date
)
