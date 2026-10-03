using Alexapps.SkinCare.Dtos.Common;
using Alexapps.SkinCare.Dtos.DiagnosticSessions.Commands;
using Alexapps.SkinCare.Dtos.DiagnosticSessions.Queries;
using Alexapps.SkinCare.Dtos.DiagnosticSessions.Results;
using Alexapps.SkinCare.Entities.Consultations;
using Alexapps.SkinCare.Entities.Doctors;
using Alexapps.SkinCare.Entities.LABs;
using Alexapps.SkinCare.Entities.Medications;
using Alexapps.SkinCare.Entities.Users;
using Alexapps.SkinCare.Enums;
using Alexapps.SkinCare.Etos;
using Alexapps.SkinCare.Integrations.Storage;
using Alexapps.SkinCare.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Users;


namespace Alexapps.SkinCare.Services
{
    [Authorize]
    public class DiagnosticSessionService : SkinCareAppService, IDiagnosticSessionService
    {
        private readonly IRepository<DiagnosticSession, Guid> _diagnosticSessionRepository;
        private readonly IRepository<DiagnosticSessionImage, Guid> _diagnosticSessionImageRepository;
        private readonly IRepository<DiagnosticSessionMessage, Guid> _diagnosticSessionMessageRepository;
        private readonly IRepository<DiagnosticSessionTest, Guid> _diagnosticSessionTestRepository;
        private readonly IRepository<DiagnosticSessionTreatmentPlan, Guid> _diagnosticSessionTreatmentPlanRepository;
        private readonly IRepository<Alexapps.SkinCare.Entities.MedicalTests.MedicalTest, Guid> _medicalTestRepository;
        private readonly IRepository<Alexapps.SkinCare.Entities.MedicalTests.SampleType, Guid> _sampleTypeRepository;
        private readonly IRepository<Medication, Guid> _medicationRepository;
        private readonly IStorageService _storageService;
        private readonly IRepository<Doctor, Guid> _doctorRepository;
        private readonly IRepository<User, Guid> _userRepository;
        private readonly ILocalEventBus _localEventBus;
        private readonly IRepository<LabMedicalTest, Guid> _labMedicalTestRepository;
        private readonly INotificationAppService _notificationAppService;
        public DiagnosticSessionService(
            IRepository<DiagnosticSession, Guid> diagnosticSessionRepository,
            IRepository<DiagnosticSessionImage, Guid> diagnosticSessionImageRepository,
            IRepository<DiagnosticSessionMessage, Guid> diagnosticSessionMessageRepository,
            IRepository<DiagnosticSessionTest, Guid> diagnosticSessionTestRepository,
            IRepository<DiagnosticSessionTreatmentPlan, Guid> diagnosticSessionTreatmentPlanRepository,
            IRepository<Alexapps.SkinCare.Entities.MedicalTests.MedicalTest, Guid> medicalTestRepository,
            IRepository<Alexapps.SkinCare.Entities.MedicalTests.SampleType, Guid> sampleTypeRepository,
            IRepository<Medication, Guid> medicationRepository,
            IStorageService storageService,
            IRepository<Doctor, Guid> doctorRepository,
            IRepository<User, Guid> userRepository,
            ILocalEventBus localEventBus, IRepository<LabMedicalTest, Guid> labMedicalTestRepository, INotificationAppService notificationAppService)
        {
            _diagnosticSessionRepository = diagnosticSessionRepository;
            _diagnosticSessionImageRepository = diagnosticSessionImageRepository;
            _diagnosticSessionMessageRepository = diagnosticSessionMessageRepository;
            _diagnosticSessionTestRepository = diagnosticSessionTestRepository;
            _diagnosticSessionTreatmentPlanRepository = diagnosticSessionTreatmentPlanRepository;
            _medicalTestRepository = medicalTestRepository;
            _sampleTypeRepository = sampleTypeRepository;
            _medicationRepository = medicationRepository;
            _storageService = storageService;
            _doctorRepository = doctorRepository;
            _userRepository = userRepository;
            _localEventBus = localEventBus;
            _labMedicalTestRepository = labMedicalTestRepository;
            _notificationAppService = notificationAppService;

        }

        public async Task<DiagnosticSessionDto> CreateAsync(CreateDiagnosticSessionDto input)
        {
            if (input.DoctorId.HasValue)
            {
                var doctorExists = await _doctorRepository.AnyAsync(x => x.Id == input.DoctorId.Value);
                if (!doctorExists)
                {
                    throw new UserFriendlyException("Doctor not found.");
                }
            }

            var session = new DiagnosticSession
            {
                UserId = CurrentUser.Id.GetValueOrDefault(),
                DoctorId = input.DoctorId,
                Status = DiagnosticSessionStatus.Pending,
                Description = input.Description,
                Duration = input.Duration,
                ProductsUsed = input.ProductsUsed,
                MedicalHistory = input.MedicalHistory,
                Allergies = input.Allergies,
                DateOfBirth = input.DateOfBirth,
                Images = new List<DiagnosticSessionImage>()
            };

            if (input.Images != null && input.Images.Any())
            {
                foreach (var imageFile in input.Images)
                {
                    var fullImageUrl = await _storageService.Upload(imageFile, "diagnostic-sessions");

                    string relativePath;
                    if (Uri.TryCreate(fullImageUrl, UriKind.Absolute, out Uri uri))
                    {
                        relativePath = uri.PathAndQuery;
                    }
                    else
                    {
                        relativePath = fullImageUrl;
                    }

                    session.Images.Add(new DiagnosticSessionImage
                    {
                        ImageUrl = relativePath
                    });
                }
            }

            await _diagnosticSessionRepository.InsertAsync(session, autoSave: true);

          
            if (session.DoctorId.HasValue)
            {
              
                var doctor = await _doctorRepository.GetAsync(session.DoctorId.Value);

                await _notificationAppService.NotifyDoctorNewBookingAsync(
                    doctor.UserId,
                    session.Id,
                    CurrentUser.Name ?? "A Patient"
                );
            }
          

            return await GetAsync(session.Id);
        }
        public async Task<DiagnosticSessionDto> GetAsync(Guid id)
        {
            var query = await _diagnosticSessionRepository.GetQueryableAsync();
            var session = await query
                .Include(x => x.User)
                .Include(x => x.Images)
                .Include(x => x.Doctor).ThenInclude(d => d.User)
                .Include(x => x.Doctor).ThenInclude(d => d.Specialty)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (session == null)
            {
                throw new UserFriendlyException("Diagnostic session not found.");
            }

            return ObjectMapper.Map<DiagnosticSession, DiagnosticSessionDto>(session);
        }

        public async Task<PagedResultWithMetadata<DiagnosticSessionDto>> GetListAsync(GetDiagnosticSessionListDto input)

        {
            // Ensure we include Images. Using WithDetailsAsync or Include if accessible.
            // Since we are using GetQueryableAsync, we might need to cast to IQueryable or use WithDetails if using repository method directly.
            // But usually we can use .Include() on IQueryable.
            // However, to use .Include(), we need Microsoft.EntityFrameworkCore namespace.

            // Allow loading with details
            // Allow loading with details
            var query = await _diagnosticSessionRepository.GetQueryableAsync();
            query = query.Include(x => x.Images)
                         .Include(x => x.Doctor).ThenInclude(d => d.User)
                         .Include(x => x.Doctor).ThenInclude(d => d.Specialty)
                         .Include(x => x.Messages).ThenInclude(m => m.DiagnosticSessionTest).ThenInclude(t => t.MedicalTest).ThenInclude(mt => mt.TestCategory);

            query = query.Where(x => x.UserId == CurrentUser.Id.GetValueOrDefault());

            if (!string.IsNullOrWhiteSpace(input.Filter))
            {
                query = query.Where(x => x.Doctor != null && x.Doctor.User.Name.Contains(input.Filter));
            }

            var totalCount = await AsyncExecuter.CountAsync(query);

            query = query.OrderByDescending(x => x.CreationTime)
                         .Skip((input.Page - 1) * input.Limit)
                         .Take(input.Limit);


            var sessions = await AsyncExecuter.ToListAsync(query);
            var dtos = ObjectMapper.Map<List<DiagnosticSession>, List<DiagnosticSessionDto>>(sessions);

            var currentUserId = CurrentUser.Id.GetValueOrDefault();
            foreach (var dto in dtos)
            {
                var session = sessions.First(s => s.Id == dto.Id);
                var messages = session.Messages;

                DateTime? lastMsgTime = null;
                string lastMsgContent = null;
                int unreadCount = 0;

                if (messages != null && messages.Any())
                {
                    var lastMessage = messages.OrderByDescending(m => m.CreationTime).First();
                    lastMsgTime = lastMessage.CreationTime;

                    if (!string.IsNullOrEmpty(lastMessage.Message))
                    {
                        lastMsgContent = lastMessage.Message;
                    }
                    else if (lastMessage.DiagnosticSessionTestId.HasValue)
                    {
                        var isArabic = System.Globalization.CultureInfo.CurrentUICulture.Name.StartsWith("ar");
                        var prefix = isArabic ? "تحليل طبي: " : "Medical Test: ";
                        lastMsgContent = $"{prefix}{lastMessage.DiagnosticSessionTest?.MedicalTest?.GetName()}";
                    }
                    else if (lastMessage.Type == DiagnosticSessionMessageType.TreatmentPlan && lastMessage.DiagnosticSessionTreatmentPlans.Any())
                    {
                        var isArabic = System.Globalization.CultureInfo.CurrentUICulture.Name.StartsWith("ar");
                        var prefix = isArabic ? "خطة علاجية: " : "Treatment Plan: ";
                        var firstPlan = lastMessage.DiagnosticSessionTreatmentPlans.First();
                        var medicationName = (isArabic ? firstPlan.Medication?.NameAr : firstPlan.Medication?.NameEn) ?? firstPlan.Medication?.GetName();
                        lastMsgContent = $"{prefix}{medicationName}";
                        if (lastMessage.DiagnosticSessionTreatmentPlans.Count > 1)
                        {
                            lastMsgContent += isArabic ? " (وآخرون)" : " (+others)";
                        }
                    }
                    else if (!string.IsNullOrEmpty(lastMessage.FileUrl))
                    {
                        var isArabic = System.Globalization.CultureInfo.CurrentUICulture.Name.StartsWith("ar");
                        lastMsgContent = isArabic ? "[ملف]" : "[File]";
                    }

                    unreadCount = messages.Count(m => !m.IsRead && m.SenderId != currentUserId);
                }

                dto.LastMessage = lastMsgContent;
                dto.LastMessageTime = lastMsgTime;
                dto.UnreadMessageCount = unreadCount;
            }

            return new PagedResultWithMetadata<DiagnosticSessionDto>(
                dtos,
                input.Page,
                input.Limit,
                totalCount
            );

        }

        public async Task<DiagnosticSessionMessageDto> SendMessageAsync(CreateDiagnosticSessionMessageDto input)
        {
            if (string.IsNullOrWhiteSpace(input.Message) && input.File == null)
            {
                throw new UserFriendlyException("Message or file is required.");
            }

            var session = await _diagnosticSessionRepository.GetAsync(input.DiagnosticSessionId);
            // Optional: Check if user is allowed to send message to this session (e.g. owner or assigned doctor)

            var message = new DiagnosticSessionMessage
            {
                DiagnosticSessionId = input.DiagnosticSessionId,
                SenderId = CurrentUser.Id.GetValueOrDefault(),
                Message = input.Message,
                IsRead = false
            };

            if (input.File != null)
            {
                var fullFileUrl = await _storageService.Upload(input.File, "diagnostic-sessions");

                string relativePath;
                if (Uri.TryCreate(fullFileUrl, UriKind.Absolute, out Uri uri))
                {
                    relativePath = uri.PathAndQuery;
                }
                else
                {
                    relativePath = fullFileUrl;
                }

                message.FileUrl = relativePath;
            }

            var hasText = !string.IsNullOrEmpty(message.Message);
            var hasFile = !string.IsNullOrEmpty(message.FileUrl);

            if (hasText && hasFile) message.Type = DiagnosticSessionMessageType.Mixed;
            else if (hasFile)
            {
                message.Type = GetMessageType(message.FileUrl);
            }
            else message.Type = DiagnosticSessionMessageType.Text;

            await _diagnosticSessionMessageRepository.InsertAsync(message, autoSave: true);

            message.DiagnosticSession = session;
            var dto = ObjectMapper.Map<DiagnosticSessionMessage, DiagnosticSessionMessageDto>(message);
            dto.IsMe = true;
            await _localEventBus.PublishAsync(new DiagnosticSessionMessageCreatedEto(session.Id, dto));
            if (session.DoctorId.HasValue)
            {
                // نحتاج جلب الـ UserId الخاص بالدكتور لإرسال الإشعار له
                var doctor = await _doctorRepository.GetAsync(session.DoctorId.Value);

                await _notificationAppService.NotifyNewChatMessageAsync(
                    doctor.UserId,         
                    CurrentUser.Id.Value,  
                    CurrentUser.Name ?? "Patient",
                    input.Message ?? "Sent a file"
                );
            }
            return dto;
        }

        private async Task<PagedResultWithMetadata<DiagnosticSessionMessageDto>> GetSessionInteractionsAsync(Guid sessionId, GetDiagnosticSessionMessagesDto input, Guid currentUserId)

        {
            var msgQuery = await _diagnosticSessionMessageRepository.GetQueryableAsync();
            msgQuery = msgQuery.Include(x => x.DiagnosticSession)
                   .Include(x => x.DiagnosticSessionTest).ThenInclude(t => t.MedicalTest).ThenInclude(mt => mt.TestCategory)
                   .Include(x => x.DiagnosticSessionTest).ThenInclude(t => t.SampleType)
                
                   .Include(x => x.DiagnosticSessionTreatmentPlans).ThenInclude(tp => tp.Medication);

            var messages = await AsyncExecuter.ToListAsync(msgQuery.Where(x => x.DiagnosticSessionId == sessionId));

            // Mark messages as read
            var unreadMessages = messages.Where(m => !m.IsRead && m.SenderId != currentUserId).ToList();
            foreach (var msg in unreadMessages)
            {
                msg.IsRead = true;
                await _diagnosticSessionMessageRepository.UpdateAsync(msg);

                if (msg.DiagnosticSessionTestId.HasValue)
                {
                    var test = await _diagnosticSessionTestRepository.GetAsync(msg.DiagnosticSessionTestId.Value);
                    test.IsRead = true;
                    await _diagnosticSessionTestRepository.UpdateAsync(test);
                }
            }

            var dtos = ObjectMapper.Map<List<DiagnosticSessionMessage>, List<DiagnosticSessionMessageDto>>(messages);
            var sortedDtos = dtos.OrderByDescending(x => x.CreationTime).ToList();

            var totalCount = sortedDtos.Count;

            var paged = sortedDtos
                .Skip((input.Page - 1) * input.Limit)
                .Take(input.Limit)
                .ToList();

            foreach (var dto in paged)
            {
                dto.IsMe = dto.SenderId == currentUserId;
            }

            return new PagedResultWithMetadata<DiagnosticSessionMessageDto>(
                paged,
                input.Page,
                input.Limit,
                totalCount
            );

        }

        public async Task<PagedResultWithMetadata<DiagnosticSessionMessageDto>> GetMessagesAsync(GetDiagnosticSessionMessagesDto input)

        {
            var currentUserId = CurrentUser.Id.GetValueOrDefault();
            return await GetSessionInteractionsAsync(input.DiagnosticSessionId, input, currentUserId);
        }

        // ─── Doctor-facing methods ────────────────────────────────────────────────

        private async Task<Doctor> GetCurrentDoctorAsync()
        {
            var doctorQuery = await _doctorRepository.GetQueryableAsync();
            var doctor = await doctorQuery.FirstOrDefaultAsync(d => d.UserId == CurrentUser.Id.GetValueOrDefault());
            if (doctor == null)
                throw new UserFriendlyException("Doctor profile not found for the current user.");
            return doctor;
        }

        public async Task<PagedResultWithMetadata<DiagnosticSessionDto>> DoctorGetListAsync(GetDiagnosticSessionListDto input)

        {
            var doctor = await GetCurrentDoctorAsync();

            var query = await _diagnosticSessionRepository.GetQueryableAsync();
            query = query.Include(x => x.Images)
                         .Include(x => x.Doctor).ThenInclude(d => d.User)
                         .Include(x => x.Doctor).ThenInclude(d => d.Specialty)
                         .Include(x => x.Messages).ThenInclude(m => m.DiagnosticSessionTest).ThenInclude(t => t.MedicalTest).ThenInclude(mt => mt.TestCategory);

            query = query.Where(x => x.DoctorId == doctor.Id);

            var totalCount = await AsyncExecuter.CountAsync(query);

            query = query.OrderByDescending(x => x.CreationTime)
                         .Skip((input.Page - 1) * input.Limit)
                         .Take(input.Limit);


            var sessions = await AsyncExecuter.ToListAsync(query);
            var dtos = ObjectMapper.Map<List<DiagnosticSession>, List<DiagnosticSessionDto>>(sessions);

            var userIds = sessions.Select(s => s.UserId).Distinct().ToList();
            var usersQuery = await _userRepository.GetQueryableAsync();
            var users = await AsyncExecuter.ToListAsync(usersQuery.Where(u => userIds.Contains(u.Id)));
            var userDict = users.ToDictionary(u => u.Id);

            var currentUserId = CurrentUser.Id.GetValueOrDefault();
            foreach (var dto in dtos)
            {
                if (userDict.TryGetValue(dto.UserId, out var user))
                {
                    dto.ClientName = user.Name;
                    dto.ClientImage = user.ProfileImage;
                }

                var session = sessions.First(s => s.Id == dto.Id);
                var messages = session.Messages;

                DateTime? lastMsgTime = null;
                string lastMsgContent = null;
                int unreadCount = 0;

                if (messages != null && messages.Any())
                {
                    var lastMessage = messages.OrderByDescending(m => m.CreationTime).First();
                    lastMsgTime = lastMessage.CreationTime;

                    if (!string.IsNullOrEmpty(lastMessage.Message))
                    {
                        lastMsgContent = lastMessage.Message;
                    }
                    else if (lastMessage.DiagnosticSessionTestId.HasValue)
                    {
                        var isArabic = System.Globalization.CultureInfo.CurrentUICulture.Name.StartsWith("ar");
                        var prefix = isArabic ? "تحليل طبي: " : "Medical Test: ";
                        lastMsgContent = $"{prefix}{lastMessage.DiagnosticSessionTest?.MedicalTest?.GetName()}";
                    }
                    else if (lastMessage.Type == DiagnosticSessionMessageType.TreatmentPlan && lastMessage.DiagnosticSessionTreatmentPlans.Any())
                    {
                        var isArabic = System.Globalization.CultureInfo.CurrentUICulture.Name.StartsWith("ar");
                        var prefix = isArabic ? "خطة علاجية: " : "Treatment Plan: ";
                        var firstPlan = lastMessage.DiagnosticSessionTreatmentPlans.First();
                        var medicationName = (isArabic ? firstPlan.Medication?.NameAr : firstPlan.Medication?.NameEn) ?? firstPlan.Medication?.GetName();
                        lastMsgContent = $"{prefix}{medicationName}";
                        if (lastMessage.DiagnosticSessionTreatmentPlans.Count > 1)
                        {
                            lastMsgContent += isArabic ? " (وآخرون)" : " (+others)";
                        }
                    }
                    else if (!string.IsNullOrEmpty(lastMessage.FileUrl))
                    {
                        var isArabic = System.Globalization.CultureInfo.CurrentUICulture.Name.StartsWith("ar");
                        lastMsgContent = isArabic ? "[ملف]" : "[File]";
                    }

                    unreadCount = messages.Count(m => !m.IsRead && m.SenderId != currentUserId);
                }

                dto.LastMessage = lastMsgContent;
                dto.LastMessageTime = lastMsgTime;
                dto.UnreadMessageCount = unreadCount;
            }

            return new PagedResultWithMetadata<DiagnosticSessionDto>(dtos, input.Page, input.Limit, totalCount);

        }

        public async Task<DiagnosticSessionDto> DoctorGetAsync(Guid id)
        {
            var doctor = await GetCurrentDoctorAsync();

            var query = await _diagnosticSessionRepository.GetQueryableAsync();
            var session = await query
                .Include(x => x.User)
                .Include(x => x.Images)
                .Include(x => x.Doctor).ThenInclude(d => d.User)
                .Include(x => x.Doctor).ThenInclude(d => d.Specialty)
                .FirstOrDefaultAsync(x => x.Id == id && x.DoctorId == doctor.Id);

            if (session == null)
                throw new UserFriendlyException("Diagnostic session not found.");

            var dto = ObjectMapper.Map<DiagnosticSession, DiagnosticSessionDto>(session);

            var userQuery = await _userRepository.GetQueryableAsync();
            var user = await AsyncExecuter.FirstOrDefaultAsync(userQuery.Where(u => u.Id == session.UserId));
            if (user != null)
            {
                dto.ClientName = user.Name;
                dto.ClientImage = user.ProfileImage;
            }

            return dto;
        }

        public async Task<DiagnosticSessionMessageDto> DoctorSendMessageAsync(CreateDiagnosticSessionMessageDto input)
        {
            // 1. Validation for input
            if (input == null) throw new ArgumentNullException(nameof(input));

            if (string.IsNullOrWhiteSpace(input.Message) && input.File == null)
            {
                throw new UserFriendlyException("Message or file is required.");
            }

            var doctor = await GetCurrentDoctorAsync();

            // 2. Defensive check for doctor
            if (doctor == null) throw new UserFriendlyException("Doctor profile not found.");

            var session = await _diagnosticSessionRepository.GetAsync(input.DiagnosticSessionId);
            if (session == null) throw new UserFriendlyException("Session not found.");

            if (session.DoctorId != doctor.Id)
                throw new UserFriendlyException("You are not assigned to this diagnostic session.");

            var msgQuery = await _diagnosticSessionMessageRepository.GetQueryableAsync();
            var doctorMessageCount = await AsyncExecuter.CountAsync(
                msgQuery.Where(m => m.DiagnosticSessionId == input.DiagnosticSessionId
                                  && m.SenderId == doctor.UserId));

            var message = new DiagnosticSessionMessage
            {
                DiagnosticSessionId = input.DiagnosticSessionId,
                SenderId = doctor.UserId, // Safely use doctor's User Id
                Message = input.Message,
                IsRead = false
            };

            if (input.File != null)
            {
                var fullFileUrl = await _storageService.Upload(input.File, "diagnostic-sessions");
                message.FileUrl = Uri.TryCreate(fullFileUrl, UriKind.Absolute, out Uri uri)
                                  ? uri.PathAndQuery
                                  : fullFileUrl;
            }

            // Determine Message Type
            var hasText = !string.IsNullOrWhiteSpace(message.Message);
            var hasFile = !string.IsNullOrWhiteSpace(message.FileUrl);

            if (hasText && hasFile) message.Type = DiagnosticSessionMessageType.Mixed;
            else if (hasFile) message.Type = GetMessageType(message.FileUrl);
            else message.Type = DiagnosticSessionMessageType.Text;

            await _diagnosticSessionMessageRepository.InsertAsync(message, autoSave: true);

            if (doctorMessageCount == 0)
            {
                session.Status = DiagnosticSessionStatus.InProgress;
                await _diagnosticSessionRepository.UpdateAsync(session, autoSave: true);
            }

            message.DiagnosticSession = session;
            var dto = ObjectMapper.Map<DiagnosticSessionMessage, DiagnosticSessionMessageDto>(message);
            dto.IsMe = true;

            await _localEventBus.PublishAsync(new DiagnosticSessionMessageCreatedEto(session.Id, dto));

            // 3. Secure Notification Block
            try
            {
                var notificationText = string.IsNullOrWhiteSpace(input.Message) ? "Sent an attachment" : input.Message;

                // Ensure name is never null to avoid downstream issues in Notification Service
                var senderDisplayName = "Doctor";
                if (doctor.User != null && !string.IsNullOrWhiteSpace(doctor.User.Name))
                {
                    senderDisplayName = doctor.User.Name;
                }

                await _notificationAppService.NotifyNewChatMessageAsync(
                    session.UserId,
                    doctor.UserId,
                    senderDisplayName,
                    notificationText
                );
            }
            catch (Exception ex)
            {
                // Log notification failure but don't break the main request
                Logger.LogException(ex);
            }

            return dto;
        }

        public async Task<DiagnosticSessionMessageDto> DoctorSendTestAsync(CreateDiagnosticSessionTestDto input)
        {
            var doctor = await GetCurrentDoctorAsync();

            var session = await _diagnosticSessionRepository.GetAsync(input.DiagnosticSessionId);
            if (session.DoctorId != doctor.Id)
                throw new UserFriendlyException("You are not assigned to this diagnostic session.");

            var testExists = await _medicalTestRepository.AnyAsync(x => x.Id == input.MedicalTestId);
            if (!testExists) throw new UserFriendlyException("Selected medical test not found.");

            var sampleExists = await _sampleTypeRepository.AnyAsync(x => x.Id == input.SampleTypeId);
            if (!sampleExists) throw new UserFriendlyException("Selected sample type not found.");

            // --- Start of Price Range Logic (Min & Max) ---
            var labMedicalTestsQuery = await _labMedicalTestRepository.GetQueryableAsync();

            // Fetch prices from labs that provide this specific medical test and are currently enabled
            var labPrices = await AsyncExecuter.ToListAsync(
                labMedicalTestsQuery.Where(lt => lt.MedicalTestId == input.MedicalTestId && lt.IsEnabled)
                                    .Select(lt => lt.LabPrice)
            );

            // Calculate minimum and maximum prices instead of the average
            double minPrice = labPrices.Any() ? labPrices.Min() : 0;
            double maxPrice = labPrices.Any() ? labPrices.Max() : 0;
            // --- End of Price Range Logic ---

            var msgQuery = await _diagnosticSessionMessageRepository.GetQueryableAsync();
            var doctorMessageCount = await AsyncExecuter.CountAsync(
                msgQuery.Where(m => m.DiagnosticSessionId == input.DiagnosticSessionId
                                  && m.SenderId == doctor.UserId));

            var sessionTest = new DiagnosticSessionTest
            {
                DiagnosticSessionId = input.DiagnosticSessionId,
                SenderId = CurrentUser.Id.GetValueOrDefault(),
                MedicalTestId = input.MedicalTestId,
                SampleTypeId = input.SampleTypeId,
                PreparationInstructions = input.PreparationInstructions,
                IsRead = false,

                // Store the price range in the entity
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                IsPaid = false
            };

            await _diagnosticSessionTestRepository.InsertAsync(sessionTest, autoSave: true);

            // Create a chat message linked to this medical test request
            var message = new DiagnosticSessionMessage
            {
                DiagnosticSessionId = sessionTest.DiagnosticSessionId,
                SenderId = sessionTest.SenderId,
                DiagnosticSessionTestId = sessionTest.Id,
                Type = DiagnosticSessionMessageType.MedicalText,
                IsRead = false
            };

            await _diagnosticSessionMessageRepository.InsertAsync(message, autoSave: true);

            // If this is the doctor's first interaction, update session status to InProgress
            if (doctorMessageCount == 0)
            {
                session.Status = DiagnosticSessionStatus.InProgress;
                await _diagnosticSessionRepository.UpdateAsync(session, autoSave: true);
            }

            // Refetch the message with all nested details for the DTO mapping
            var messageQueryRefetched = await _diagnosticSessionMessageRepository.GetQueryableAsync();
            var savedMessage = await AsyncExecuter.FirstOrDefaultAsync(
                messageQueryRefetched.Include(m => m.DiagnosticSessionTest).ThenInclude(t => t.MedicalTest).ThenInclude(mt => mt.TestCategory)
                                     .Include(m => m.DiagnosticSessionTest).ThenInclude(t => t.SampleType)
                                     .Where(m => m.Id == message.Id)
            );

            var dto = ObjectMapper.Map<DiagnosticSessionMessage, DiagnosticSessionMessageDto>(savedMessage);
            dto.Status = session.Status.ToString();
            dto.IsMe = true;
            await _notificationAppService.NotifyPatientMedicalTestRequestedAsync(
        session.UserId,
        session.Id,
        sessionTest.Id
    );
            await _localEventBus.PublishAsync(new DiagnosticSessionMessageCreatedEto(session.Id, dto));
            return dto;
        }
   
        public async Task<DiagnosticSessionMessageDto> DoctorSendTreatmentPlanAsync(Guid sessionId)
        {
            var doctor = await GetCurrentDoctorAsync();

          
            var session = await _diagnosticSessionRepository.GetAsync(sessionId);
            if (session.DoctorId != doctor.Id)
                throw new UserFriendlyException("You are not assigned to this diagnostic session.");

           
            var draftItems = await _diagnosticSessionTreatmentPlanRepository.GetListAsync(x =>
                x.DiagnosticSessionId == session.Id && x.DiagnosticSessionMessageId == null);

            if (!draftItems.Any())
                throw new UserFriendlyException("Please add at least one medication before sending.");

           
            var message = new DiagnosticSessionMessage
            {
                DiagnosticSessionId = session.Id,
                SenderId = doctor.UserId,
                Type = DiagnosticSessionMessageType.TreatmentPlan,
                IsRead = false
            };

           
            await _diagnosticSessionMessageRepository.InsertAsync(message, autoSave: true);

           
            foreach (var item in draftItems)
            {
                item.DiagnosticSessionMessageId = message.Id;
                await _diagnosticSessionTreatmentPlanRepository.UpdateAsync(item);
            }

          
            var msgQuery = await _diagnosticSessionMessageRepository.GetQueryableAsync();
            var doctorMessageCount = await AsyncExecuter.CountAsync(
                msgQuery.Where(m => m.DiagnosticSessionId == session.Id && m.SenderId == doctor.UserId));

            if (doctorMessageCount <= 1) 
            {
                session.Status = DiagnosticSessionStatus.InProgress;
                await _diagnosticSessionRepository.UpdateAsync(session, autoSave: true);
            }

           
            var messageQueryRefetched = await _diagnosticSessionMessageRepository.GetQueryableAsync();
            var savedMessage = await AsyncExecuter.FirstOrDefaultAsync(
                messageQueryRefetched
                    .Include(m => m.DiagnosticSessionTreatmentPlans)
                        .ThenInclude(tp => tp.Medication)
                    .Where(m => m.Id == message.Id)
            );

            var dto = ObjectMapper.Map<DiagnosticSessionMessage, DiagnosticSessionMessageDto>(savedMessage);
            dto.Status = session.Status.ToString();
            dto.IsMe = true;

            await _notificationAppService.NotifyPatientTreatmentPlanCreatedAsync(
        session.UserId,
        session.Id,
        message.Id
    );
            await _localEventBus.PublishAsync(new DiagnosticSessionMessageCreatedEto(session.Id, dto));

            return dto;
        }
        public async Task<DiagnosticSessionMessageTreatmentPlanDto> AddTreatmentPlanItemAsync(AddIndividualTreatmentPlanDto input)
        {
            var doctor = await GetCurrentDoctorAsync();
            var session = await _diagnosticSessionRepository.GetAsync(input.DiagnosticSessionId);

            if (session.DoctorId != doctor.Id)
                throw new UserFriendlyException("You are not assigned to this session.");

            var medication = await _medicationRepository.GetAsync(input.MedicationId);

            var treatmentPlan = new DiagnosticSessionTreatmentPlan
            {
                DiagnosticSessionId = session.Id,
                SenderId = doctor.UserId,
                MedicationId = input.MedicationId,
                Dosage = input.Dosage,
                Frequency = input.Frequency,
                Duration = input.Duration,
                Quantity = input.Quantity,
                Refills = input.Refills,
                SpecialInstructions = input.SpecialInstructions,
                DiagnosticSessionMessageId = null
            };

            await _diagnosticSessionTreatmentPlanRepository.InsertAsync(treatmentPlan, autoSave: true);

            var dto = ObjectMapper.Map<DiagnosticSessionTreatmentPlan, DiagnosticSessionMessageTreatmentPlanDto>(treatmentPlan);
            var currentCulture = System.Globalization.CultureInfo.CurrentUICulture.Name;

            dto.MedicationName = currentCulture.StartsWith("ar")
      ? medication.NameAr
      : medication.NameEn;

            return dto;
        }
        public async Task DeleteTreatmentPlanItemAsync(Guid id)
        {
            var item = await _diagnosticSessionTreatmentPlanRepository.GetAsync(id);

            if (item.DiagnosticSessionMessageId != null)
                throw new UserFriendlyException("Cannot delete medication that has already been sent.");

            await _diagnosticSessionTreatmentPlanRepository.DeleteAsync(id);
        }
        public async Task<List<DiagnosticSessionMessageTreatmentPlanDto>> GetAddedTreatmentPlanItemsAsync(Guid sessionId)
        {
            var query = await _diagnosticSessionTreatmentPlanRepository.GetQueryableAsync();

            var items = await AsyncExecuter.ToListAsync(
                query.Include(x => x.Medication)
                     .Where(x => x.DiagnosticSessionId == sessionId && x.DiagnosticSessionMessageId == null)
            );

            return ObjectMapper.Map<List<DiagnosticSessionTreatmentPlan>, List<DiagnosticSessionMessageTreatmentPlanDto>>(items);
        }
        public async Task<BookTreatmentPlanResultDto> BookTreatmentPlanAsync(BookTreatmentPlanDto input)
        {
            var userId = CurrentUser.GetId();

           
            var treatmentPlans = await _diagnosticSessionTreatmentPlanRepository.GetListAsync(x =>
                x.DiagnosticSessionMessageId == input.DiagnosticSessionMessageId &&
                x.DiagnosticSession.UserId == userId);

            if (!treatmentPlans.Any())
                throw new UserFriendlyException("No medications found for this message.");

            var sharedOrderNumber = $"ORD-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";

            foreach (var plan in treatmentPlans)
            {
                plan.OrderNumber = sharedOrderNumber;
                plan.IsPaid = true;
                plan.Status = TreatmentPlanStatus.Preparing;
                plan.Latitude = input.Latitude;
                plan.Longitude = input.Longitude;
                plan.DetailedAddress = input.DetailedAddress;

                await _diagnosticSessionTreatmentPlanRepository.UpdateAsync(plan);
            }

            await CurrentUnitOfWork.SaveChangesAsync();
            return new BookTreatmentPlanResultDto { OrderNumber = sharedOrderNumber };
        }
        public async Task<DiagnosticSessionMessageTestResultDto> DoctorSendTestResultAsync(SendTestResultDto input)
        {
            var doctor = await GetCurrentDoctorAsync();

            var queryable = await _diagnosticSessionTestRepository.GetQueryableAsync();
            // جلب الاختبار مع بياناته الطبية
            var test = await queryable
                .Include(t => t.MedicalTest)
                .FirstOrDefaultAsync(t => t.Id == input.DiagnosticSessionTestId);

            if (test == null) throw new UserFriendlyException("Test not found.");

            var session = await _diagnosticSessionRepository.GetAsync(test.DiagnosticSessionId);
            if (session.DoctorId != doctor.Id)
                throw new UserFriendlyException("You are not assigned to this diagnostic session.");

            if (input.ResultFile == null)
                throw new UserFriendlyException("Result file is required.");

            var msgQuery = await _diagnosticSessionMessageRepository.GetQueryableAsync();
            var doctorMessageCount = await AsyncExecuter.CountAsync(
                msgQuery.Where(m => m.DiagnosticSessionId == session.Id
                                  && m.SenderId == doctor.UserId));

            var fullFileUrl = await _storageService.Upload(input.ResultFile, "test-results");

            test.ResultFileUrl = Uri.TryCreate(fullFileUrl, UriKind.Absolute, out Uri uri)
                                  ? uri.PathAndQuery
                                  : fullFileUrl;

            var cleanFileNameFromPayload = !string.IsNullOrWhiteSpace(input.ResultFileName)
                                           ? System.IO.Path.GetFileName(input.ResultFileName)
                                           : null;

            var cleanFileNameFromFile = System.IO.Path.GetFileName(input.ResultFile.FileName);
            test.ResultFileName = cleanFileNameFromPayload ?? cleanFileNameFromFile;

            await _diagnosticSessionTestRepository.UpdateAsync(test, autoSave: true);

            var message = new DiagnosticSessionMessage
            {
                DiagnosticSessionId = test.DiagnosticSessionId,
                SenderId = CurrentUser.Id.GetValueOrDefault(),
                DiagnosticSessionTestId = test.Id,
                Type = DiagnosticSessionMessageType.TestResult,
                Message = null,
                FileUrl = test.ResultFileUrl,
                IsRead = false
            };

            // حفظ الرسالة
            await _diagnosticSessionMessageRepository.InsertAsync(message, autoSave: true);

            if (doctorMessageCount == 0)
            {
                session.Status = DiagnosticSessionStatus.InProgress;
                await _diagnosticSessionRepository.UpdateAsync(session, autoSave: true);
            }

            message.DiagnosticSessionTest = test;

            var chatDto = ObjectMapper.Map<DiagnosticSessionMessage, DiagnosticSessionMessageDto>(message);
            chatDto.IsMe = true;
            chatDto.Status = session.Status.ToString();

            
            await _localEventBus.PublishAsync(new DiagnosticSessionMessageCreatedEto(session.Id, chatDto));

           
            await _notificationAppService.NotifyPatientTestResultUploadedAsync(
                session.UserId,
                session.Id,
                test.Id,
                test.MedicalTest?.GetName() ?? "Test"
            );

            return new DiagnosticSessionMessageTestResultDto
            {
                DiagnosticSessionTestId = test.Id,
                TestName = test.MedicalTest?.GetName(),
                ResultFileUrl = test.ResultFileUrl,
                ResultFileName = test.ResultFileName
            };
        }
        public async Task<PagedResultWithMetadata<DiagnosticSessionMessageDto>> DoctorGetMessagesAsync(GetDiagnosticSessionMessagesDto input)

        {
            var doctor = await GetCurrentDoctorAsync();

            var session = await _diagnosticSessionRepository.GetAsync(input.DiagnosticSessionId);
            if (session.DoctorId != doctor.Id)
                throw new UserFriendlyException("You are not assigned to this diagnostic session.");

            var currentUserId = CurrentUser.Id.GetValueOrDefault();
            return await GetSessionInteractionsAsync(input.DiagnosticSessionId, input, currentUserId);
        }
        public async Task<DiagnosticSessionFullDetailsDto> GetSessionFullDetailsAsync(Guid sessionId, GetDiagnosticSessionDetailsDto input)
        {
            var query = await _diagnosticSessionRepository.GetQueryableAsync();

            var session = await query
    .Include(s => s.Tests).ThenInclude(t => t.MedicalTest)
  
    .Include(s => s.TreatmentPlans).ThenInclude(tp => tp.Medication)
  
    .Include(s => s.Messages).ThenInclude(m => m.DiagnosticSessionTreatmentPlans).ThenInclude(tp => tp.Medication)
    .AsNoTracking()
    .FirstOrDefaultAsync(s => s.Id == sessionId);

            if (session == null) throw new UserFriendlyException("Session not found");

            var dto = ObjectMapper.Map<DiagnosticSession, DiagnosticSessionFullDetailsDto>(session);

          
            if (input.IncludeSessionFiles)
            {
                var mediaTypes = new[] {
            DiagnosticSessionMessageType.Image,
            DiagnosticSessionMessageType.Video,
            DiagnosticSessionMessageType.File,
            DiagnosticSessionMessageType.Mixed
        };

                dto.SessionFiles = session.Messages
                    .Where(m => mediaTypes.Contains(m.Type) && !string.IsNullOrEmpty(m.FileUrl))
                    .OrderByDescending(m => m.CreationTime)
                    .Select(m => new DiagnosticSessionFileDto
                    {
                        MessageId = m.Id,
                        FileUrl = m.FileUrl,
                        FileName = System.IO.Path.GetFileName(m.FileUrl),
                        Type = m.Type,
                        CreationTime = m.CreationTime
                    })
                    .ToList();
            }
            else
            {
                dto.SessionFiles = new List<DiagnosticSessionFileDto>();
            }


            dto.TotalTestsCount = dto.Tests?.Count ?? 0;
            if (input.IncludeTests && dto.Tests != null)
            {
                dto.Tests = dto.Tests
                    .Skip((input.Page - 1) * input.Limit)
                    .Take(input.Limit)
                    .ToList();
            }
            else
            {
                dto.Tests = new List<DiagnosticSessionFullTestDto>();
            }

            // 3. معالجة الأدوية
            if (!input.IncludeTreatmentPlans)
            {
                dto.TreatmentPlans = new List<DiagnosticSessionMessageTreatmentPlanDto>();
            }

            return dto;
        }

        private DiagnosticSessionMessageType GetMessageType(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl))
                return DiagnosticSessionMessageType.File;

            // Strip query parameters and fragments if it's a URL
            var path = fileUrl.Split('?')[0].Split('#')[0];
            var ext = System.IO.Path.GetExtension(path).ToLower();

            // Explicitly handle document types to ensure they are categorized as File
            var documentExtensions = new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt" };
            if (documentExtensions.Contains(ext))
                return DiagnosticSessionMessageType.File;

            var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp", ".heic" };
            var videoExtensions = new[] { ".mp4", ".mov", ".avi", ".webm", ".mkv" };

            if (imageExtensions.Contains(ext))
                return DiagnosticSessionMessageType.Image;

            if (videoExtensions.Contains(ext))
                return DiagnosticSessionMessageType.Video;

            return DiagnosticSessionMessageType.File;
        }
    }
}

