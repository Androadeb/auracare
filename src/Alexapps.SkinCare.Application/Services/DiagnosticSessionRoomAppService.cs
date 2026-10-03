using Alexapps.SkinCare.Configurations;
using Alexapps.SkinCare.Dtos.DiagnosticSessions.Results;
using Alexapps.SkinCare.Entities.Consultations;
using Alexapps.SkinCare.Enums;
using Alexapps.SkinCare.Etos;
using Alexapps.SkinCare.Interfaces;
using Alexapps.SkinCare.Localization;
using JWT.Algorithms;
using JWT.Builder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Localization;

namespace Alexapps.SkinCare.Services
{
    [Authorize] // Added Authorization for the whole service
    public class DiagnosticSessionRoomAppService : ApplicationService, IDiagnosticSessionRoomAppService
    {
        private readonly IRepository<DiagnosticSessionRoom, Guid> _sessionRoomRepository;
        private readonly IRepository<DiagnosticSession, Guid> _diagnosticSessionRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly VideoSdkConfiguration _videoSdkConfig;
        private readonly IConfiguration _configuration;
        private readonly IRepository<DiagnosticSessionMessage, Guid> _sessionMessageRepository;
        private readonly ILocalEventBus _localEventBus;
        public DiagnosticSessionRoomAppService(
            IRepository<DiagnosticSessionRoom, Guid> sessionRoomRepository,
            IRepository<DiagnosticSession, Guid> diagnosticSessionRepository,
            IHttpClientFactory httpClientFactory,
            VideoSdkConfiguration videoSdkConfig,
            IConfiguration configuration,
            IRepository<DiagnosticSessionMessage, Guid> sessionMessageRepository,
            ILocalEventBus localEventBus)
        {
            LocalizationResource = typeof(SkinCareResource);
            _sessionRoomRepository = sessionRoomRepository;
            _diagnosticSessionRepository = diagnosticSessionRepository;
            _httpClientFactory = httpClientFactory;
            _videoSdkConfig = videoSdkConfig;
            _configuration = configuration;
            _sessionMessageRepository = sessionMessageRepository;
            _localEventBus = localEventBus;
        }

        public async Task<DiagnosticSessionRoomDto> CreateMeetingRoomAsync(Guid diagnosticSessionId, DateTime startTime)
        {
            var session = await _diagnosticSessionRepository.GetAsync(diagnosticSessionId);

            // 1. ضمان أن الوقت القادم هو UTC وموسوم كـ UTC صريح
            var startTimeUtc = DateTime.SpecifyKind(startTime.ToUniversalTime(), DateTimeKind.Utc);

            // 2. استخدام UtcNow للمقارنة والعمليات الحسابية
            var nowUtc = DateTime.UtcNow;

            var queryable = await _sessionRoomRepository.GetQueryableAsync();

            var existingRoom = await queryable
                .Where(x => x.DiagnosticSessionId == diagnosticSessionId)
                .OrderByDescending(x => x.CreationTime)
                .FirstOrDefaultAsync();

            // 3. نمرر وقت البداية الـ UTC (الذي أصبح موسوماً بالفعل)
            var token = GenerateVideoSdkToken(startTimeUtc);

            if (existingRoom != null &&
                existingRoom.Status != VideoRoomStatus.Ended &&
                existingRoom.Status != VideoRoomStatus.Cancelled &&
                existingRoom.ScheduledEndTime > nowUtc)
            {
                var dto = ObjectMapper.Map<DiagnosticSessionRoom, DiagnosticSessionRoomDto>(existingRoom);
                dto.Token = token;
                return dto;
            }

            var roomId = await CreateRoomFromVideoSdkAsync();
            var appDomain = _configuration["App:SelfUrl"];

            var sessionRoom = new DiagnosticSessionRoom
            {
                DiagnosticSessionId = session.Id,
                VideoRoomId = roomId,

                // 4. الحفظ في القاعدة كـ UTC صريح
                ScheduledStartTime = startTimeUtc,
                ScheduledEndTime = startTimeUtc.AddMinutes(60),

                Status = VideoRoomStatus.AppointmentReserved,
                Token = token,
                DoctorJoinUrl = $"{appDomain}/meeting/join/{roomId}",
                PatientJoinUrl = $"{appDomain}/meeting/join/{roomId}"
            };

            await _sessionRoomRepository.InsertAsync(sessionRoom, autoSave: true);

            var resultDto = ObjectMapper.Map<DiagnosticSessionRoom, DiagnosticSessionRoomDto>(sessionRoom);
            resultDto.Token = token;
            resultDto.VideoRoomId = roomId;

            return resultDto;
        }

        public async Task<RoomValidationResultDto> ValidateRoomAsync(string roomId)
        {
            if (string.IsNullOrEmpty(roomId))
                return new RoomValidationResultDto { IsValid = false, Message = "Room ID is missing", ErrorCode = "INVALID_ID" };

            var room = await _sessionRoomRepository.FirstOrDefaultAsync(x => x.VideoRoomId == roomId);

            if (room == null)
                return new RoomValidationResultDto { IsValid = false, Message = "Room not found", ErrorCode = "NOT_FOUND" };

            // فحص هل الوقت الحالي تجاوز وقت نهاية الجلسة المجدول
            if (DateTime.UtcNow > room.ScheduledEndTime)
            {
                // تحديث الحالة في قاعدة البيانات لتصبح منتهية
                if (room.Status != VideoRoomStatus.Ended)
                {
                    room.Status = VideoRoomStatus.Ended;
                    await _sessionRoomRepository.UpdateAsync(room, autoSave: true);
                }

                return new RoomValidationResultDto
                {
                    IsValid = false,
                    Message = "The session token has expired",
                    ErrorCode = "TOKEN_EXPIRED" // الموبايل هيفهم من الكود ده إن الوقت خلص
                };
            }

            // التحقق من الطرف الثالث VideoSDK
            var client = _httpClientFactory.CreateClient("VideoSdkClient");
            var token = GenerateVideoSdkToken();
            client.DefaultRequestHeaders.Remove("Authorization");
            client.DefaultRequestHeaders.Add("Authorization", token);

            var response = await client.GetAsync($"rooms/validate/{roomId}");

            if (!response.IsSuccessStatusCode)
            {
                return new RoomValidationResultDto { IsValid = false, Message = "Room is invalid on VideoSDK", ErrorCode = "VIDEOSDK_INVALID" };
            }

            return new RoomValidationResultDto { IsValid = true, Message = "Room is active", ErrorCode = "SUCCESS" };
        }
        public async Task EndActiveSessionAsync(string roomId)
        {
            // 1. نكلم VideoSDK عشان نقفل الجلسة تقنياً (دي إنت عاملها صح)
            var client = _httpClientFactory.CreateClient("VideoSdkClient");
            var token = GenerateVideoSdkToken();
            client.DefaultRequestHeaders.Remove("Authorization");
            client.DefaultRequestHeaders.Add("Authorization", token);

            var response = await client.PostAsJsonAsync("sessions/end", new { roomId = roomId });

            if (response.IsSuccessStatusCode)
            {
                var room = await _sessionRoomRepository.FirstOrDefaultAsync(x => x.VideoRoomId == roomId);
                if (room != null)
                {
                    
                    room.Status = VideoRoomStatus.Ended;
                    await _sessionRoomRepository.UpdateAsync(room);
                }
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new UserFriendlyException($"Failed to end session: {error}");
            }
        }

        private async Task<string> CreateRoomFromVideoSdkAsync()
        {
            var client = _httpClientFactory.CreateClient("VideoSdkClient");
            var token = GenerateVideoSdkToken();

           
            client.DefaultRequestHeaders.Remove("Authorization"); 
            client.DefaultRequestHeaders.Add("Authorization", token);

         
            System.Diagnostics.Debug.WriteLine($"Sending Token: {token}");

            var response = await client.PostAsJsonAsync("rooms", new { /* payload */ });

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<VideoSdkResponse>();
                return data?.VideoRoomId;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Volo.Abp.UserFriendlyException($"Video SDK Error: {errorContent}");
            }
        }

        private string GenerateVideoSdkToken(DateTime? scheduledStartTime = null)
        {
            var apiKey = _videoSdkConfig.ApiKey;
            var secretKey = _videoSdkConfig.SecretKey;

       
            var iat = new DateTimeOffset(DateTime.UtcNow).AddSeconds(-60).ToUnixTimeSeconds();

            long exp;
            if (scheduledStartTime.HasValue)
            {
         
                var startTimeUtc = DateTime.SpecifyKind(scheduledStartTime.Value.ToUniversalTime(), DateTimeKind.Utc);
                exp = new DateTimeOffset(startTimeUtc).AddMinutes(60).ToUnixTimeSeconds();
            }
            else
            {
                exp = new DateTimeOffset(DateTime.UtcNow).AddHours(1).ToUnixTimeSeconds();
            }

            var token = JwtBuilder.Create()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(secretKey)
                .AddClaim("apikey", apiKey)
                .AddClaim("permissions", new string[] { "allow_join", "allow_mod" })
                .AddClaim("iat", iat)
                .AddClaim("exp", exp)
                .AddClaim("version", 2)
                .Encode();

            return token;
        }
        // ميثود داخل DiagnosticSessionRoomAppService
        [AllowAnonymous]
        [RemoteService(false)]
        public async Task AutoEndRoomInDbAsync(string roomId)
        {
            var room = await _sessionRoomRepository.FirstOrDefaultAsync(x => x.VideoRoomId == roomId);

            if (room != null && room.Status != VideoRoomStatus.Ended)
            {
                room.Status = VideoRoomStatus.Ended;
                await _sessionRoomRepository.UpdateAsync(room, autoSave: true);

                Logger.LogInformation($"Room {roomId} has been automatically marked as Ended after 1 hour.");
            }
        }

        public async Task SendBookingConfirmationAsync(Guid sessionId, DateTime scheduledTimeUtc)
        {

            var isoDateStr = scheduledTimeUtc.ToString("yyyy-MM-ddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture);

            var message = new DiagnosticSessionMessage
            {
                DiagnosticSessionId = sessionId,
                SenderId = Guid.Empty,
                Message = isoDateStr,
                Type = DiagnosticSessionMessageType.BookingConfirmation,
                IsRead = false
            };

            await _sessionMessageRepository.InsertAsync(message, autoSave: true);

            var dto = ObjectMapper.Map<DiagnosticSessionMessage, DiagnosticSessionMessageDto>(message);
            dto.IsMe = false;

            await _localEventBus.PublishAsync(new DiagnosticSessionMessageCreatedEto(sessionId, dto));
        }
        [AllowAnonymous]
        [RemoteService(false)]
        public async Task SendVideoCallLinkToChatAsync(Guid sessionId, string roomId, DateTime scheduledTime)
        {
            
            var token = GenerateVideoSdkToken(scheduledTime);

         
            var appDomain = _configuration["App:SelfUrl"] ?? "http://localhost:4200";

           
            var directJoinUrl = $"{appDomain}/meeting/join/{roomId}?token={token}";

            var message = new DiagnosticSessionMessage
            {
                DiagnosticSessionId = sessionId,
                SenderId = Guid.Empty,
                Message = "Your video session is ready. Click to join!",
                Type = DiagnosticSessionMessageType.VideoCall,

              
                RoomId = roomId,
                Token = token,

                
                FileUrl = directJoinUrl,
                IsRead = false
            };

           
            await _sessionMessageRepository.InsertAsync(message, autoSave: true);

          
            var dto = ObjectMapper.Map<DiagnosticSessionMessage, DiagnosticSessionMessageDto>(message);
            dto.IsMe = false;
            dto.Token = token;
            dto.RoomId = roomId;
            dto.FileUrl = directJoinUrl;

            await _localEventBus.PublishAsync(new DiagnosticSessionMessageCreatedEto(sessionId, dto));
        }
    }

    public class VideoSdkResponse
    {
        [System.Text.Json.Serialization.JsonPropertyName("roomId")]
        public string VideoRoomId { get; set; }
    }
}