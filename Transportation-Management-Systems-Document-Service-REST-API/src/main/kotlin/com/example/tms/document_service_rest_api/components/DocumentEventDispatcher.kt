package com.example.tms.document_service_rest_api.components

import com.example.tms.document_service_rest_api.configurations.RabbitMQConfig
import com.example.tms.document_service_rest_api.models.sealedmodels.TMSDocumentEvent
import org.springframework.amqp.rabbit.core.BatchingRabbitTemplate
import org.springframework.amqp.rabbit.core.RabbitTemplate
import org.springframework.beans.factory.annotation.Qualifier
import org.springframework.stereotype.Component

/**
 * ## Document Event Dispatcher
 *
 * This is a component class. This class is represented to calculate or create a specific message to be created with
 * a routing key to a specific exchange before it goes into the queue in RabbitMQ
 *
 * Exchange - The component in RabbitMQ that helps filter which message that has routing key goes into the queue
 * Routing key - It's the address of the message that categorizes where it goes and how other messages differentiates with each other
 * Object - The message itself to send to the queue
 */
@Component
class DocumentEventDispatcher(
    @Qualifier("rabbitTemplate") private val rabbitTemplate: RabbitTemplate,
    private val batchingRabbitTemplate: BatchingRabbitTemplate
) {
    fun dispatchSingleMessageToRabbitMQ(actionSubTopic: String, dataforQueue: TMSDocumentEvent) {
        val specificRoutingKeyFor = "tms.document.$actionSubTopic"

        rabbitTemplate.convertAndSend(
            RabbitMQConfig.EXCHANGE_NAME,
            specificRoutingKeyFor,
            dataforQueue
        )
    }

    fun dispatchMultipleMessageToRabbitMQ(actionSubTopic: String, multipleDataForQueue: List<TMSDocumentEvent>) {
        val specificRoutingKeyFor = "tms.document.$actionSubTopic"

        multipleDataForQueue.forEach { event ->
            batchingRabbitTemplate.convertAndSend(
                RabbitMQConfig.EXCHANGE_NAME,
                specificRoutingKeyFor,
                event
            )
        }
    }
}