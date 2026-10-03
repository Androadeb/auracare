using Alexapps.SkinCare.Dtos.DiagnosticSessions.Results;
using Alexapps.SkinCare.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;

namespace Alexapps.SkinCare.Controllers
{
    [Route("api/v1/diagnostic-sessions")]
    public class DiagnosticSessionRoomController : AbpController
    {
        private readonly IDiagnosticSessionRoomAppService _videoRoomService;

        public DiagnosticSessionRoomController(IDiagnosticSessionRoomAppService videoRoomService)
        {
            _videoRoomService = videoRoomService;
        }

        // GET: api/v1/diagnostic-sessions/validate-room/{roomId}
        [HttpGet]
        [Route("validate-room/{roomId}")]
        public async Task<RoomValidationResultDto> ValidateRoomAsync(string roomId)
        {
          
            return await _videoRoomService.ValidateRoomAsync(roomId);
        }

        // POST: api/v1/diagnostic-sessions/end-active-room/{roomId}
        [HttpPost]
        [Route("end-active-room/{roomId}")]
        public async Task EndActiveSessionAsync(string roomId)
        {
            await _videoRoomService.EndActiveSessionAsync(roomId);
        }
    }
}
