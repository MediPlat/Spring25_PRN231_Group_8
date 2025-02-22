using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediPlat.Model.ResponseObject.Auth
{
    public class OTPResponse
    {
        public string? accountId { get; set; }
        public string otp { get; set; } = null!;
    }
}
