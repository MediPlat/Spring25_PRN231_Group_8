using MediPlat.Model.Authen_Athor;
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
    }
}
