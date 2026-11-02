package com.example.tms.document_service_rest_api.configurations

import io.swagger.v3.oas.models.OpenAPI
import io.swagger.v3.oas.models.info.Contact
import io.swagger.v3.oas.models.info.Info
import org.springframework.context.annotation.Bean
import org.springframework.context.annotation.Configuration

@Configuration
class SwaggerOpenApiConfig {
    @Bean
    fun customOpenAPI(): OpenAPI {
        return OpenAPI()
            .info(
                Info().title("Transportation Management System Document Service REST API").description(
                    "This application is created for managing documents which are meant for the order shipments for customers & employees to manage"
                ).contact(
                    Contact()
                        .name("Adrian Joseph")
                        .url("https://www.linkedin.com/in/adrianjoseph15/")
                        .email("adrianignatius15@hotmail.com")

                ).summary("This application is treated as a service in Kubernetes microservices")
            )
    }
}