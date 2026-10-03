using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Controllers
{
    [Route("api/debug/logs")]
    public class LogViewerController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetLogs(int lines = 100)
        {
            // ABP بيكتب اللوجات غالباً في فولدر Logs
            var logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", "logs.txt");

            if (!System.IO.File.Exists(logFilePath))
            {
                return NotFound("Log file not found.");
            }

            // قراءة الملف مع السماح لعمليات تانية بالكتابة فيه (FileShare.ReadWrite)
            using var stream = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = new StreamReader(stream);

            var allLines = await reader.ReadToEndAsync();
            var lastLines = allLines.Split('\n').TakeLast(lines);

            return Content(string.Join("\n", lastLines), "text/plain; charset=utf-8");
        }
    }
}
