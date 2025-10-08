using Microsoft.Extensions.Hosting;
using OrderMicroService.BLL.RabbitMQ;

namespace OrderMicroService.BLL.RabbitMQ;

public class RabbitMQProductDeletionHostedService : IHostedService
{
  private readonly IRabbitMQProductDeleteConsumer _productDelettionConsumer;

  public RabbitMQProductDeletionHostedService(IRabbitMQProductDeleteConsumer consumer)
  {
    _productDelettionConsumer = consumer;
  }


  public Task StartAsync(CancellationToken cancellationToken)
  {
    _productDelettionConsumer.Consume();

    return Task.CompletedTask;
  }

  public Task StopAsync(CancellationToken cancellationToken)
  {
    _productDelettionConsumer.Dispose();

    return Task.CompletedTask;
  }
}
