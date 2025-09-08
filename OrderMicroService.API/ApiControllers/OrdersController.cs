using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using OrderMicroService.BLL.DTO;
using OrderMicroService.BLL.ServiceContract;
using OrderMicroService.DAL.Entities;

namespace OrderMicroService.API.ApiControllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IOrdersService _ordersService;

    public OrdersController(IOrdersService ordersService)
    {
            _ordersService = ordersService;
    }
    [HttpGet("Test")]
    public IActionResult Test()
    {
        return Ok("Hello Order");
    }
    [HttpGet]
    public async Task<IEnumerable<OrderResponse>> Get()
    {
        List<OrderResponse?> orders=await _ordersService.GetOrders();
        return orders;
    }
    //GET: /api/Orders/search/orderid/{orderID}
    [HttpGet("GetOrderByOrderID")]
    public async Task<OrderResponse?> GetOrderByOrderID(Guid orderID)
    {
        FilterDefinition<Order> filter = Builders<Order>.Filter.Eq(temp => temp.OrderID, orderID);

        OrderResponse? order = await _ordersService.GetOrderByCondition(filter);
        return order;
    }


    //GET: /api/Orders/search/productid/{productID}
    [HttpGet("GetOrdersByProductID")]
    public async Task<IEnumerable<OrderResponse?>> GetOrdersByProductID(Guid productID)
    {
        FilterDefinition<Order> filter = Builders<Order>.Filter.ElemMatch(temp => temp.OrderItems,
          Builders<OrderItem>.Filter.Eq(tempProduct => tempProduct.ProductID, productID)
          );

        List<OrderResponse?> orders = await _ordersService.GetOrdersByCondition(filter);
        return orders;
    }


    //GET: /api/Orders/search/orderDate/{orderDate}
    [HttpGet("GetOrdersByOrderDate")]
    public async Task<IEnumerable<OrderResponse?>> GetOrdersByOrderDate(DateTime orderDate)
    {
        FilterDefinition<Order> filter = Builders<Order>.Filter.Eq(temp => temp.OrderDate.ToString("yyyy-MM-dd"), orderDate.ToString("yyyy-MM-dd")
          );

        List<OrderResponse?> orders = await _ordersService.GetOrdersByCondition(filter);
        return orders;
    }
}
