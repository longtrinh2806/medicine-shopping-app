package com.trinhkimlong.authentication.publish;

import com.trinhkimlong.authentication.contracts.UserCreated;
import org.springframework.amqp.rabbit.core.RabbitTemplate;
import org.springframework.stereotype.Service;

@Service
public class RabbitMQProducer {
    private String exchange = "user-created-exchange";
    private String routingKey = "user_routing_key";
    private RabbitTemplate rabbitTemplate;

    public RabbitMQProducer(RabbitTemplate rabbitTemplate) {
        this.rabbitTemplate = rabbitTemplate;
    }
    public void sendJsonMessage(UserCreated userCreated) {
        rabbitTemplate.convertAndSend(exchange, routingKey, userCreated);
    }
}
