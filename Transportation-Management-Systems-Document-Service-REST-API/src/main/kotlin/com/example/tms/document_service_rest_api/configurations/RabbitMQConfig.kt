package com.example.tms.document_service_rest_api.configurations

import org.springframework.amqp.core.Binding
import org.springframework.amqp.core.BindingBuilder
import org.springframework.amqp.core.Queue
import org.springframework.amqp.core.TopicExchange
import org.springframework.amqp.rabbit.batch.SimpleBatchingStrategy
import org.springframework.amqp.rabbit.config.SimpleRabbitListenerContainerFactory
import org.springframework.amqp.rabbit.connection.ConnectionFactory
import org.springframework.amqp.rabbit.core.BatchingRabbitTemplate
import org.springframework.amqp.rabbit.core.RabbitTemplate
import org.springframework.amqp.support.converter.JacksonJsonMessageConverter
import org.springframework.beans.factory.annotation.Qualifier
import org.springframework.context.annotation.Bean
import org.springframework.context.annotation.Configuration
import org.springframework.scheduling.concurrent.ThreadPoolTaskScheduler

@Configuration
class RabbitMQConfig {
    companion object {
        const val TMS_SHIPMENT_DOCUMENT_QUEUE_NAME: String = "tms-document"
        const val TMS_ORDER_EVENT_QUEUE_NAME: String = "tms-order"

        const val SHIPMENT_DOCUMENT_EXCHANGE_NAME: String = "shipment.documents.exchange"
        const val SHIPMENT_DOCUMENT_ROUTING_KEY: String = "tms.document.#"

        const val ORDER_EVENT_EXCHANGE_NAME: String = "order.event.exchange"
        const val ORDER_EVENT_ROUTING_KEY: String = "order.event.#"
    }

    @Bean
    fun shipmentDocumentQueue(): Queue {
        return Queue(TMS_SHIPMENT_DOCUMENT_QUEUE_NAME, true)
    }

    @Bean
    fun orderEventQueue(): Queue {
        return Queue(TMS_ORDER_EVENT_QUEUE_NAME, true)
    }

    @Bean
    fun shipmentDocumentExchange(): TopicExchange {
        return TopicExchange(SHIPMENT_DOCUMENT_EXCHANGE_NAME)
    }

    @Bean
    fun orderEventExchange(): TopicExchange {
        return TopicExchange(ORDER_EVENT_EXCHANGE_NAME)
    }

    @Bean
    fun orderEventBinding(
        @Qualifier("orderEventQueue") queue: Queue,
        @Qualifier("orderEventExchange") exchange: TopicExchange
    ): Binding {
        return BindingBuilder.bind(queue).to(exchange).with(ORDER_EVENT_ROUTING_KEY)
    }

    @Bean
    fun shipmentDocumentBinding(
        @Qualifier("shipmentDocumentQueue") queue: Queue,
        @Qualifier("shipmentDocumentExchange") exchange: TopicExchange
    ): Binding {
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

    @Bean
    fun rabbitListenerContainerFactory(
        connectionFactory: ConnectionFactory,
        messageConverter: JacksonJsonMessageConverter
    ): SimpleRabbitListenerContainerFactory {
        return SimpleRabbitListenerContainerFactory().apply {
            this.setConnectionFactory(connectionFactory)
            this.setMessageConverter(messageConverter)
        }
    }
}