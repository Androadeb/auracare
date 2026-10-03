using Alexapps.SkinCare.Dtos.Common;
using Alexapps.SkinCare.Dtos.Doctors.Results;
using Alexapps.SkinCare.Dtos.Doctors.Queries;
using Alexapps.SkinCare.Interfaces;
using Alexapps.SkinCare.Routes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Controllers
{
    public class DoctorController(IDoctorService doctorService) : SkinCareController
    {
        [HttpGet(ApiRoutes.Doctors.Base)]
        [AllowAnonymous]
        public async Task<PagedResultWithMetadata<DoctorDto>> GetListAsync([FromQuery] GetDoctorsInput input)
        {
            return await doctorService.GetListAsync(input);
        }

        [HttpGet(ApiRoutes.Doctors.Single)]
        [AllowAnonymous]
        public async Task<DoctorDto> GetAsync(System.Guid id)
        {
            return await doctorService.GetAsync(id);
        }

        [HttpGet(ApiRoutes.Doctors.Specialties)]
        [AllowAnonymous]
        public async Task<List<SpecialtyDto>> GetSpecialtiesAsync()
        {
            return await doctorService.GetSpecialtiesAsync();
        }

     
    }
}
