using Alexapps.SkinCare.Dtos.Doctors.Commands;
using Alexapps.SkinCare.Dtos.Doctors.Results;
using Alexapps.SkinCare.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace Alexapps.SkinCare.Controllers
{
    [RemoteService(Name = "Qualification")]
    [Area("app")]
    [ControllerName("Qualification")]
    // تأكدنا أن المسار يحتوي على كلمة doctor لكي يلتقطه الـ Predicate في الـ Module ويضعه في الـ Doctor API
    [Route("api/v1/doctor/qualifications")]
    [Authorize]
    public class QualificationController : AbpController, IQualificationAppService
    {
        private readonly IQualificationAppService _service;

        public QualificationController(IQualificationAppService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("my")] 
        public virtual Task<List<DoctorQualificationDto>> GetMyQualificationsAsync()
        {
            return _service.GetMyQualificationsAsync();
        }

        [HttpPost]
        [Route("")] 
        [Consumes("multipart/form-data")]
        public virtual Task<DoctorQualificationDto> CreateAsync([FromForm] UpdateQualificationDto input)
        {
            return _service.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")] 
        [Consumes("multipart/form-data")]
        public virtual Task<DoctorQualificationDto> UpdateAsync(Guid id, [FromForm] UpdateQualificationDto input)
        {
            return _service.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")] 
        public virtual Task DeleteAsync(Guid id)
        {
            return _service.DeleteAsync(id);
        }
    }
}