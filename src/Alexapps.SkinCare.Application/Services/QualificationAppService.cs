using Alexapps.SkinCare.Dtos.Doctors.Commands;
using Alexapps.SkinCare.Dtos.Doctors.Results;
using Alexapps.SkinCare.Entities.Doctors;
using Alexapps.SkinCare.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace Alexapps.SkinCare.Services
{
    public class QualificationAppService : SkinCareAppService, IQualificationAppService
    {
        private readonly IRepository<DoctorQualification, Guid> _repository;
        private readonly IRepository<Doctor, Guid> _doctorRepository;

        public QualificationAppService(
            IRepository<DoctorQualification, Guid> repository,
            IRepository<Doctor, Guid> doctorRepository)
        {
            _repository = repository;
            _doctorRepository = doctorRepository;
        }

     
        private async Task<Guid> GetCurrentDoctorIdAsync()
        {
            var userId = CurrentUser.GetId(); 
            var doctor = await _doctorRepository.FirstOrDefaultAsync(d => d.UserId == userId);

            if (doctor == null)
            {
                throw new UserFriendlyException("Doctor profile not found for the current user.");
            }

            return doctor.Id;
        }

      
        public async Task<List<DoctorQualificationDto>> GetMyQualificationsAsync()
        {
            var doctorId = await GetCurrentDoctorIdAsync();
            var qualifications = await _repository.GetListAsync(q => q.DoctorId == doctorId);

            return ObjectMapper.Map<List<DoctorQualification>, List<DoctorQualificationDto>>(qualifications);
        }

        public async Task<DoctorQualificationDto> CreateAsync(UpdateQualificationDto input)
        {
          
            var doctorId = await GetCurrentDoctorIdAsync();

            var qualification = new DoctorQualification
            {
                DoctorId = doctorId,
                Degree = input.Degree,
                CertificateImageUrl = await UploadImageAsync(input.CertificateImage)
            };

            await _repository.InsertAsync(qualification, autoSave: true);
            return ObjectMapper.Map<DoctorQualification, DoctorQualificationDto>(qualification);
        }

        public async Task<DoctorQualificationDto> UpdateAsync(Guid id, UpdateQualificationDto input)
        {
            var doctorId = await GetCurrentDoctorIdAsync();
            var qualification = await _repository.GetAsync(id);

           
            if (qualification.DoctorId != doctorId)
            {
                throw new UserFriendlyException("You are not authorized to update this qualification.");
            }

            qualification.Degree = input.Degree;

            if (input.CertificateImage != null)
            {
                qualification.CertificateImageUrl = await UploadImageAsync(input.CertificateImage);
            }
            else
            {
                qualification.CertificateImageUrl = input.ExistingImageUrl;
            }

            await _repository.UpdateAsync(qualification, autoSave: true);
            return ObjectMapper.Map<DoctorQualification, DoctorQualificationDto>(qualification);
        }

        public async Task DeleteAsync(Guid id)
        {
            var doctorId = await GetCurrentDoctorIdAsync();
            var qualification = await _repository.GetAsync(id);

          
            if (qualification.DoctorId != doctorId)
            {
                throw new UserFriendlyException("You are not authorized to delete this qualification.");
            }

            await _repository.DeleteAsync(id);
        }

        private async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file == null) return "/assets/img/default-cert.jpg";

        
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "qualifications");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

         
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

       
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

           
            return "/uploads/qualifications/" + uniqueFileName;
        }
    }
}