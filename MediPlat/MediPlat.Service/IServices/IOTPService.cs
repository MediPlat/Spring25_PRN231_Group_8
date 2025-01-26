using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediPlat.Service.IServices
{
    public interface IOTPService
    {
        Task<string> SendOTPByMail(string email);

    }
}
