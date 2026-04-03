using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCodeAttendanceSystem.Data;
using QRCodeAttendanceSystem.Models;
using QRCoder;

namespace QRCodeAttendanceSystem.Controllers
{
    public class AttendanceSessionsController : Controller
    {
        private readonly AppDbContext _context;

        public AttendanceSessionsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UserId") == null)
                return RedirectToAction("Login", "Account");

            var sessions = await _context.AttendanceSessions
                .Include(s => s.CreatedByUser)
                .ToListAsync();

            return View(sessions);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserId") == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AttendanceSession session)
        {
            if (HttpContext.Session.GetString("UserId") == null)
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? "Dữ liệu không hợp lệ" : e.ErrorMessage)
                    .ToList();

                return Content("Lỗi ModelState: " + string.Join(" | ", errors));
            }

            int userId = int.Parse(HttpContext.Session.GetString("UserId")!);

            session.QRCodeValue = Guid.NewGuid().ToString();
            session.CreatedAt = DateTime.Now;
            session.IsActive = true;
            session.CreatedBy = userId;

            _context.AttendanceSessions.Add(session);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (HttpContext.Session.GetString("UserId") == null)
                return RedirectToAction("Login", "Account");

            var session = await _context.AttendanceSessions.FindAsync(id);
            if (session == null)
            {
                return NotFound();
            }

            return View(session);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AttendanceSession session)
        {
            if (HttpContext.Session.GetString("UserId") == null)
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? "Dữ liệu không hợp lệ" : e.ErrorMessage)
                    .ToList();

                return Content("Lỗi ModelState: " + string.Join(" | ", errors));
            }

            var existingSession = await _context.AttendanceSessions.FindAsync(session.SessionId);
            if (existingSession == null)
            {
                return NotFound();
            }

            existingSession.SessionName = session.SessionName;
            existingSession.SessionDate = session.SessionDate;
            existingSession.StartTime = session.StartTime;
            existingSession.EndTime = session.EndTime;
            existingSession.LateThreshold = session.LateThreshold;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (HttpContext.Session.GetString("UserId") == null)
                return RedirectToAction("Login", "Account");

            var session = await _context.AttendanceSessions
                .Include(s => s.CreatedByUser)
                .FirstOrDefaultAsync(s => s.SessionId == id);

            if (session == null)
            {
                return NotFound();
            }

            return View(session);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("UserId") == null)
                return RedirectToAction("Login", "Account");

            var session = await _context.AttendanceSessions.FindAsync(id);
            if (session == null)
            {
                return NotFound();
            }

            _context.AttendanceSessions.Remove(session);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult GenerateQRCode(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return BadRequest("QR value is empty.");
            }

            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrData = qrGenerator.CreateQrCode(value, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new PngByteQRCode(qrData);
                byte[] qrCodeBytes = qrCode.GetGraphic(20);

                return File(qrCodeBytes, "image/png");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (HttpContext.Session.GetString("UserId") == null)
                return RedirectToAction("Login", "Account");

            var session = await _context.AttendanceSessions
                .Include(s => s.CreatedByUser)
                .FirstOrDefaultAsync(s => s.SessionId == id);

            if (session == null)
                return NotFound();

            var records = await _context.AttendanceRecords
                .Include(r => r.User)
                .Where(r => r.SessionId == id)
                .OrderBy(r => r.CheckInTime)
                .ToListAsync();

            ViewBag.Session = session;
            return View(records);
        }
    }
}