﻿
namespace OrderMicroService.BLL.DTO;

public record UserDTO(Guid UserID, string? Email, string? PersonName, string Gender);
