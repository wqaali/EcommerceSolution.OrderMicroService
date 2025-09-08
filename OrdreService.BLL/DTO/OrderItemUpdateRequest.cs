using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderMicroService.BLL.DTO;
public record OrderItemUpdateRequest(Guid ProductID, decimal UnitPrice, int Quantity)
{
    public OrderItemUpdateRequest() : this(default, default, default)
    {
    }
}
