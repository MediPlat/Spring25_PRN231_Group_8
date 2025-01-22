using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediPlat.Repository.Entities
{
    public class MediPlatDBContext : DbContext
    {
        public MediPlatDBContext(DbContextOptions options)
            : base(options)
        {
        }
    }
}
