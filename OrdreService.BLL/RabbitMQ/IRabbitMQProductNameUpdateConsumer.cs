namespace OrderMicroService.BLL.RabbitMQ;

public interface IRabbitMQProductNameUpdateConsumer
{
    void Consume();
    void Dispose();
}