using Alexapps.SkinCare.Dtos.Doctors.Commands;
using Alexapps.SkinCare.Dtos.Doctors.Results;
using Alexapps.SkinCare.Interfaces;
using Alexapps.SkinCare.Routes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace Alexapps.SkinCare.Controllers
{
    [RemoteService(Name = "DoctorRating")]
    [Area("app")]
    [Route(ApiRoutes.DoctorRatings.Base)] 
    public class DoctorRatingController : AbpController, IDoctorRatingAppService
    {
        private readonly IDoctorRatingAppService _ratingAppService;

        public DoctorRatingController(IDoctorRatingAppService ratingAppService)
        {
            _ratingAppService = ratingAppService;
        }

        // POST: api/v1/doctor-ratings/{id}/complete
        [HttpPost]
        [Route("/api/v1/doctor/diagnostic-sessions/{id}/complete")] 
        public virtual Task CompleteSessionAsync(Guid id)
        {
            return _ratingAppService.CompleteSessionAsync(id);
        }

        // POST: api/v1/doctor-ratings/
        [HttpPost]
        [Route("/api/v1/client/doctor-ratings")] 
        public virtual Task CreateRatingAsync(CreateDoctorRatingDto input)
        {
            return _ratingAppService.CreateRatingAsync(input);
        }

        // GET: api/v1/client/doctor-ratings/doctor/{doctorId}
        [HttpGet]
        [Route("/api/v1/client/doctor-ratings/{doctorId}")]
        public virtual Task<List<DoctorRatingDto>> GetDoctorRatingsAsync(Guid doctorId)
        {
            return _ratingAppService.GetDoctorRatingsAsync(doctorId);
        }
    }
}
