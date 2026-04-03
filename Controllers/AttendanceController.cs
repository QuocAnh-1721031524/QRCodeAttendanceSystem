using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCodeAttendanceSystem.Data;
using QRCodeAttendanceSystem.Models;

namespace QRCodeAttendanceSystem.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly AppDbContext _context;

        public AttendanceController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult CheckIn()
        {
            if (HttpContext.Session.GetString("UserId") == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CheckIn(string qrCodeValue)
        {
            if (HttpContext.Session.GetString("UserId") == null)
                return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(qrCodeValue))
            {
                ViewBag.Message = "Vui lòng nhập mã QR.";
                return View();
            }

            qrCodeValue = qrCodeValue.Trim();

            var session = await _context.AttendanceSessions
                .FirstOrDefaultAsync(s => s.QRCodeValue == qrCodeValue);

            if (session == null)
            {
                ViewBag.Message = "Không tìm thấy buổi chấm công.";
                return View();
            }

            int userId = int.Parse(HttpContext.Session.GetString("UserId")!);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                ViewBag.Message = "Không tìm thấy người dùng.";
                return View();
            }

            var existingRecord = await _context.AttendanceRecords
                .FirstOrDefaultAsync(a => a.SessionId == session.SessionId && a.UserId == userId);

            if (existingRecord != null)
            {
                ViewBag.Message = "Bạn đã check-in rồi.";
                return View();
            }

            DateTime now = DateTime.Now;

            if (now < session.StartTime)
            {
                ViewBag.Message = "Chưa tới giờ check-in.";
                return View();
            }

            if (now > session.EndTime)
            {
                ViewBag.Message = "Buổi chấm công đã hết giờ.";
                return View();
            }

            DateTime lateTime = session.StartTime.AddMinutes(session.LateThreshold);
            string status = now <= lateTime ? "OnTime" : "Late";

            var record = new AttendanceRecord
            {
                SessionId = session.SessionId,
                UserId = userId,
                CheckInTime = now,
                Status = status,
                CreatedAt = now
            };

            _context.AttendanceRecords.Add(record);
            await _context.SaveChangesAsync();

            ViewBag.Message = $"Check-in thành công. Trạng thái: {status}";
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> History()
        {
            if (HttpContext.Session.GetString("UserId") == null)
                return RedirectToAction("Login", "Account");

            int userId = int.Parse(HttpContext.Session.GetString("UserId")!);

            var records = await _context.AttendanceRecords
                .Include(a => a.AttendanceSession)
                .Include(a => a.User)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.CheckInTime)
                .ToListAsync();

            return View(records);
        }
    }
}