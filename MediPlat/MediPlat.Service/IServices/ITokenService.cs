using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MediPlat.Service.IServices
{
    public interface ITokenService
    {
        string GenerateAccessToken(string accountId, string role, string otp);
    }
}
