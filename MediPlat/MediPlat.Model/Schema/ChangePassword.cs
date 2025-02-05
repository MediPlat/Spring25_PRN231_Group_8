using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediPlat.Model.Schema
{
    public class ChangePassword
    {
        public Guid Id { get; set; }

        public string? Old_Password { get; set; }

        public string New_Password { get; set; }
        
        public string Comfirm_Password { get; set;}
    }
    
}
