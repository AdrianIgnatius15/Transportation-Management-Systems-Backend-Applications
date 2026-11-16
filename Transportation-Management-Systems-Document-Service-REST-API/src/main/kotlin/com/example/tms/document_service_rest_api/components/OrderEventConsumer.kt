package com.example.tms.document_service_rest_api.components

import com.example.tms.document_service_rest_api.configurations.RabbitMQConfig
import com.example.tms.document_service_rest_api.models.messages.OrderCreatedEvent
import io.github.oshai.kotlinlogging.KotlinLogging
import org.springframework.amqp.rabbit.annotation.RabbitListener
import org.springframework.stereotype.Component

@Component
class OrderEventConsumer {

    companion object {
        private val logger = KotlinLogging.logger { }
    }

    @RabbitListener(queues = [RabbitMQConfig.TMS_SHIPMENT_DOCUMENT_QUEUE_NAME])
    fun receiveOrderEventMessages(message: OrderCreatedEvent) {
        logger.info {
            message.orderNumber + "received from MQ"
        }
        logger.info {
            "The message event received from queue ${message.clientId}"
        }
    }
}