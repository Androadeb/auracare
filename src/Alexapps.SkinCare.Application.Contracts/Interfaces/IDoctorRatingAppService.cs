using Alexapps.SkinCare.Dtos.Doctors.Commands;
using Alexapps.SkinCare.Dtos.Doctors.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Alexapps.SkinCare.Interfaces
{
    public interface IDoctorRatingAppService : IApplicationService
    {
      
        Task CompleteSessionAsync(Guid sessionId);

        Task CreateRatingAsync(CreateDoctorRatingDto input);
       Task<List<DoctorRatingDto>> GetDoctorRatingsAsync(Guid doctorId);
    }
}
