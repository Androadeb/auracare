using Alexapps.SkinCare.Dtos.Doctors.Commands;
using Alexapps.SkinCare.Dtos.Doctors.Results;
using Alexapps.SkinCare.Entities.Consultations;
using Alexapps.SkinCare.Entities.Doctors;
using Alexapps.SkinCare.Enums;
using Alexapps.SkinCare.Interfaces;
using AutoMapper.Internal.Mappers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace Alexapps.SkinCare.Services
{
    public class DoctorRatingAppService : ApplicationService, IDoctorRatingAppService
    {
        private readonly IRepository<DiagnosticSession, Guid> _sessionRepository;
        private readonly IRepository<Doctor, Guid> _doctorRepository;
        private readonly IRepository<DoctorRating, Guid> _ratingRepository;
        private readonly IRepository<DiagnosticSessionMessage, Guid> _messageRepository;

        public DoctorRatingAppService(
            IRepository<DiagnosticSession, Guid> sessionRepository,
            IRepository<Doctor, Guid> doctorRepository,
            IRepository<DoctorRating, Guid> ratingRepository,
            IRepository<DiagnosticSessionMessage, Guid> messageRepository)
        {
            _sessionRepository = sessionRepository;
            _doctorRepository = doctorRepository;
            _ratingRepository = ratingRepository;
            _messageRepository = messageRepository;
        }

        public async Task CompleteSessionAsync(Guid sessionId)
        {
            var session = await _sessionRepository.GetAsync(sessionId);

            if (session.Status == DiagnosticSessionStatus.Completed)
            {
                throw new UserFriendlyException("Session is already completed.");
            }

            // 1. Update session status
            session.Status = DiagnosticSessionStatus.Completed;
            await _sessionRepository.UpdateAsync(session);

            // 2. Send automated feedback request message to the chat
            var feedbackMessage = new DiagnosticSessionMessage
            {
                DiagnosticSessionId = sessionId,
                SenderId = CurrentUser.GetId(), // Usually the doctor
                Message = "Please rate the doctor's performance.",
                Type = DiagnosticSessionMessageType.FeedbackRequest,
                IsRead = false
            };

            await _messageRepository.InsertAsync(feedbackMessage);
        }

        public async Task CreateRatingAsync(CreateDoctorRatingDto input)
        {
            var session = await _sessionRepository.GetAsync(input.DiagnosticSessionId);

            // Security check: only the patient assigned to this session can rate
            if (session.UserId != CurrentUser.GetId())
            {
                throw new UserFriendlyException("You are not authorized to rate this session.");
            }

            if (session.Status != DiagnosticSessionStatus.Completed)
            {
                throw new UserFriendlyException("You can only rate after the session is completed.");
            }
            if (session.IsRated)
            {
                throw new UserFriendlyException("You have already rated this session.");
            }
            if (!session.DoctorId.HasValue)
            {
                throw new UserFriendlyException("No doctor assigned to this session.");
            }

            // 1. Save the detailed rating
            var rating = ObjectMapper.Map<CreateDoctorRatingDto, DoctorRating>(input);
            rating.DoctorId = session.DoctorId.Value;
            rating.UserId = CurrentUser.GetId();

            await _ratingRepository.InsertAsync(rating);

            // 2. Update Doctor's overall rating and review count
            var doctor = await _doctorRepository.GetAsync(session.DoctorId.Value);

            double totalStars = (doctor.Rating * doctor.NumberOfReviews) + input.Stars;
            doctor.NumberOfReviews++;

          
            double rawRating = totalStars / doctor.NumberOfReviews;
            doctor.Rating = Math.Round(rawRating * 2, MidpointRounding.AwayFromZero) / 2;

            session.IsRated = true;
            await _doctorRepository.UpdateAsync(doctor);
        }
        public async Task<List<DoctorRatingDto>> GetDoctorRatingsAsync(Guid doctorId)
        {
            
            var queryable = await _ratingRepository.GetQueryableAsync();

         
            var ratings = await AsyncExecuter.ToListAsync(
                queryable.Include(x => x.User)
                         .Where(x => x.DoctorId == doctorId)
                         .OrderByDescending(x => x.CreationTime)
            );

          
            return ObjectMapper.Map<List<DoctorRating>, List<DoctorRatingDto>>(ratings);
        }
    }
}
