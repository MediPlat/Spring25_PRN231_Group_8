using MediPlat.Model.Authen_Athor;
using MediPlat.Model.Authen_Author;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediPlat.Service.IServices
{
    public interface IAuthService
    {
        public AuthResult Login(LoginModel loginModel);
        public Task RegisterAsync(RegisterModel registerModel);
    }
}
