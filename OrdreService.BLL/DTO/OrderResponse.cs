﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderMicroService.BLL.DTO;
public record OrderResponse(Guid OrderID, Guid UserID, decimal TotalBill, DateTime OrderDate, List<OrderItemResponse> OrderItems)
{
    public OrderResponse() : this(default, default, default, default, default)
    {
    }
}
