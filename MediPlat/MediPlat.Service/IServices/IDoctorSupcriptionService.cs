using MediPlat.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediPlat.Service.IServices
{
    public interface IDoctorSupcriptionService
    {
      public Task<List<DoctorSubscription>> GetDoctorSubcriptions(Guid id);
    }
}
