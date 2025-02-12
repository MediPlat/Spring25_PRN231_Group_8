using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediPlat.Model.ResponseObject.Auth
{
    public class AuthTokensResponse
    {
        public required string AccessToken { get; set; }

        public required string Role { get; set; }
    }
}
