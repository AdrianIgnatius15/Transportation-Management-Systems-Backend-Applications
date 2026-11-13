package com.example.tms.document_service_rest_api.configurations

import org.springframework.amqp.core.Binding
import org.springframework.amqp.core.BindingBuilder
import org.springframework.amqp.core.Queue
import org.springframework.amqp.core.TopicExchange
import org.springframework.amqp.rabbit.batch.SimpleBatchingStrategy
import org.springframework.amqp.rabbit.connection.ConnectionFactory
import org.springframework.amqp.rabbit.core.BatchingRabbitTemplate
import org.springframework.amqp.rabbit.core.RabbitTemplate
import org.springframework.amqp.support.converter.JacksonJsonMessageConverter
import org.springframework.context.annotation.Bean
import org.springframework.context.annotation.Configuration
import org.springframework.scheduling.concurrent.ThreadPoolTaskScheduler

@Configuration
class RabbitMQConfig {
    companion object {
        const val QUEUE_NAME: String = "tms"

        const val SHIPMENT_DOCUMENT_EXCHANGE_NAME: String = "shipment.documents.exchange"
        const val SHIPMENT_DOCUMENT_ROUTING_KEY: String = "tms.document.#"

        const val ORDER_EVENT_EXCHANGE_NAME: String = "order.event.exchange"
        const val ORDER_EVENT_ROUTING_KEY: String = "order.event.#"
    }

    @Bean
    fun queue(): Queue {
        return Queue(QUEUE_NAME, true)
    }

    @Bean
    fun shipmentDocumentExchange(): TopicExchange {
        return TopicExchange(SHIPMENT_DOCUMENT_EXCHANGE_NAME)
    }

    @Bean
    fun orderEventExchange(): TopicExchange {
        return TopicExchange(ORDER_EVENT_EXCHANGE_NAME)
    }

    fun orderEventBinding(queue: Queue, exchange: TopicExchange): Binding {
        return BindingBuilder.bind(queue).to(exchange).with(ORDER_EVENT_ROUTING_KEY)
    }

    @Bean
    fun shipmentDocumentBinding(queue: Queue, exchange: TopicExchange): Binding {
        return BindingBuilder.bind(queue).to(exchange).with(SHIPMENT_DOCUMENT_ROUTING_KEY)
    }

    @Bean
    fun messageConverter(): JacksonJsonMessageConverter {
        return JacksonJsonMessageConverter()
    }

    @Bean
    fun rabbitBatchScheduler(): ThreadPoolTaskScheduler {
        return ThreadPoolTaskScheduler().apply {
            poolSize = 1
            setThreadNamePrefix("rabbit-batch-")
        }
    }

    @Bean
    fun rabbitTemplate(connectionFactory: ConnectionFactory): RabbitTemplate {
        val template = RabbitTemplate(connectionFactory)
        template.messageConverter = messageConverter()
        return template
    }

    @Bean
    fun batchingRabbitTemplate(
        connectionFactory: ConnectionFactory,
        rabbitBatchScheduler: ThreadPoolTaskScheduler
    ): BatchingRabbitTemplate {
        val template = BatchingRabbitTemplate(
            SimpleBatchingStrategy(10, 1_000_000, 1_000),
            rabbitBatchScheduler
        )
        template.connectionFactory = connectionFactory
        template.messageConverter = messageConverter()

        return template
    }
}