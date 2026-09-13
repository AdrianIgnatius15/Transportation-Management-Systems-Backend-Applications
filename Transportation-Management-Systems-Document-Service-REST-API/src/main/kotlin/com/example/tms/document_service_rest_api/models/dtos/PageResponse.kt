package com.example.tms.document_service_rest_api.models.dtos

data class PageResponse<T>(
    val data: List<T> = emptyList(),
    val page: Int,
    val size: Int,
    val hasNext: Boolean = false
)
