using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderMicroService.BLL.Policies;

public interface IUserServicePolicies
{
    IAsyncPolicy<HttpResponseMessage> GetCombinedPolicy();
}
