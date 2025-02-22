using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediPlat.Model.RequestObject.Auth
{
    public class ChangePasswordRequest
    {
        public string oldPassword { get; set; } = null!;
        public string newPassword { get; set; } = null!;
        public string confirmNewPassword { get; set; } = null!;
    }
}
