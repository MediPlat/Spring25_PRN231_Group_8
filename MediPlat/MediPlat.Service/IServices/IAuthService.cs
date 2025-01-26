using MediPlat.Model.ResponseObject.Auth;
using MediPlat.Model.RequestObject.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediPlat.Service.IServices
{
    public interface IAuthService
    {
        Task<AuthTokensResponse> Login(LoginRequest request);
        Task RegisterAccount(RegisterRequest request);
    }
}
