using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediPlat.Model.Schema
{
    public class ChangePassword
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string? Old_Password { get; set; }
        [Required]
        public string New_Password { get; set; }
        [Required]
        public string Comfirm_Password { get; set;}
    }
    
}
