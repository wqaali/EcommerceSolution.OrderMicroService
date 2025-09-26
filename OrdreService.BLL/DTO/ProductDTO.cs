using OrderMicroService.BLL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderMicroService.BLL.DTO;

public record ProductDTO(Guid ProductID, string? ProductName, CategoryOptions Category, double UnitPrice, int QuantityInStock
);
