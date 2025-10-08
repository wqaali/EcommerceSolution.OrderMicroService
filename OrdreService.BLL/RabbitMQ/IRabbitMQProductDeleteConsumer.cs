namespace OrderMicroService.BLL.RabbitMQ;

public interface IRabbitMQProductDeleteConsumer
{
    void Consume();
    void Dispose();
}