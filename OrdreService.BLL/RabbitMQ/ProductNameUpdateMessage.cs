namespace OrderMicroService.BLL.RabbitMQ;

public record ProductNameUpdateMessage(Guid ProductID, string? NewName);
