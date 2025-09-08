﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderMicroService.BLL.DTO;

public record OrderItemAddRequest(Guid ProductID, decimal UnitPrice, int Quantity)
{
    public OrderItemAddRequest() : this(default, default, default)
    {
    }
}
