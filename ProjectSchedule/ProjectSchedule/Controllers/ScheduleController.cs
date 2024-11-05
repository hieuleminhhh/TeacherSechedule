using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ProjectSchedule.Dto;
using ProjectSchedule.Models;
using System.Globalization;
using System.Linq;
using System.Web;

[ApiController]
[Route("api/Schedule")]
public class ScheduleController : ControllerBase
{
    private readonly ProjectPRN231Context _context;

    public ScheduleController(ProjectPRN231Context context)
    {
        _context = context;
    }

    [HttpPost("RegisterSchedule")]
    public IActionResult RegisterSchedule([FromBody] CreateScheduleDTO s)
    {
        if (s == null || s.SubjectClassId <= 0 || s.TeacherId <= 0 || s.SlotId <= 0 || string.IsNullOrEmpty(s.DayOfWeek))
        {
            return BadRequest("Invalid request: One or more required fields are missing or invalid");
        }
        var slotExists = _context.TimeSlots.Any(slot => slot.SlotId == s.SlotId);
        if (!slotExists)
        {
            return BadRequest("Invalid request: SlotId does not exist");
        }
        var subjectClassExists = _context.SubjectClasses.Any(subjectClass => subjectClass.SubjectClassId == s.SubjectClassId);
        if (!subjectClassExists)
        {
            return BadRequest("Invalid request: SubjectClassId does not exist");
        }

        var teacherExists = _context.Teachers.Any(teacher => teacher.TeacherId == s.TeacherId);
        if (!teacherExists)
        {
            return BadRequest("Invalid request: TeacherId does not exist");
        }
        var validDayOfWeek = Enum.IsDefined(typeof(DayOfWeek), s.DayOfWeek);
        if (!validDayOfWeek)
        {
            return BadRequest("Invalid request: DayOfWeek is invalid");
        }

        var existingSchedule = _context.Schedules.FirstOrDefault(schedule =>
        schedule.SlotId == s.SlotId &&
        schedule.Date == s.Date &&
        (schedule.TeacherId == s.TeacherId));


        if (existingSchedule != null)
        {
            return BadRequest("Invalid request: There is already a schedule for this slot on this day");
        }

        var newSchedule = new Schedule
        {
            ScheduleId = s.ScheduleId,
            SubjectClassId = s.SubjectClassId,
            TeacherId = s.TeacherId,
            SlotId = s.SlotId,
            Date = s.Date,
            DayOfWeek = s.DayOfWeek,
        };
        _context.Schedules.Add(newSchedule);
        _context.SaveChanges();

        return Ok(new { Message = "Schedule registered successfully" });
    }
    /*[HttpGet]
    [Route("/CheckAvailability")]
    public IActionResult CheckAvailability(int teacherId, int slotId,)
    {



        return Ok(new
        {
            isTeacherAvailable = isTeacherAvailable
        });
    }*/

    [HttpGet("GetSchedule")]
    public IActionResult GetSchedule()
    {
        var schedules = _context.Schedules
            .Include(c => c.SubjectClass)
            .Include(t => t.Teacher)
            .Include(s => s.Slot)
            .Select(se => new GetScheduleDTO
            {
                ScheduleId = se.ScheduleId,
                SubjectClassId = se.SubjectClassId,
                TeacherId = se.TeacherId,
                TeacherName = $"{se.Teacher.FirstName} {se.Teacher.LastName}",
                SlotId = se.SlotId,
                StartTime = se.Slot.StartTime,
                EndTime = se.Slot.EndTime,
                Date = se.Date,
                DayOfWeek = se.DayOfWeek,
                SubjectCode = se.SubjectClass.Subject.SubjectCode,
                ClassName = se.SubjectClass.Class.ClassName
            })
            .ToList();
        return Ok(schedules);
    }
    [HttpGet("GetScheduleByTeacher/{teacherId}")]
    public IActionResult GetScheduleByTeacher(int teacherId)
    {
        var today = DateTime.UtcNow.Date;

        TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        var vietnamToday = TimeZoneInfo.ConvertTimeFromUtc(today, vietnamTimeZone);

        var startDate = vietnamToday.AddDays(-(int)vietnamToday.DayOfWeek).AddDays(1);

        var endDate = startDate.AddDays(6);

        var schedules = _context.Schedules
            .Include(c => c.SubjectClass)
            .Include(t => t.Teacher)
            .Include(s => s.Slot)
            .Where(se => se.TeacherId == teacherId && se.Date >= startDate && se.Date <= endDate)
            .Select(se => new GetScheduleDTO
            {
                ScheduleId = se.ScheduleId,
                SubjectClassId = se.SubjectClassId,
                TeacherId = se.TeacherId,
                TeacherName = $"{se.Teacher.FirstName} {se.Teacher.LastName}",
                SlotId = se.SlotId,
                StartTime = se.Slot.StartTime,
                EndTime = se.Slot.EndTime,
                Date = se.Date,
                DayOfWeek = se.DayOfWeek,
                SubjectCode = se.SubjectClass.Subject.SubjectCode,
                ClassName = se.SubjectClass.Class.ClassName
            })
            .ToList();
        return Ok(schedules);

    }
    [HttpGet("GetScheduleBydd/MM/yyyy/{teacherId}/{startDate}/{endDate}/{year}")]
    public IActionResult GetScheduleByTeacher(int teacherId, string startDate, string endDate, int year)
    {
        startDate = HttpUtility.UrlDecode(startDate);
        endDate = HttpUtility.UrlDecode(endDate);

        startDate += "/" + year.ToString();
        endDate += "/" + year.ToString();

        var startDateTime = DateTime.ParseExact(startDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        var endDateTime = DateTime.ParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
        var startYear = startDateTime.Year;

        var schedules = _context.Schedules
            .Include(c => c.SubjectClass)
            .Include(t => t.Teacher)
            .Include(s => s.Slot)
            .Where(se => se.TeacherId == teacherId && se.Date >= startDateTime && se.Date <= endDateTime && (se.Date.HasValue ? se.Date.Value.Year : 0) == year)
            .Select(se => new GetScheduleDTO
            {
                ScheduleId = se.ScheduleId,
                SubjectClassId = se.SubjectClassId,
                TeacherId = se.TeacherId,
                TeacherName = $"{se.Teacher.FirstName} {se.Teacher.LastName}",
                SlotId = se.SlotId,
                StartTime = se.Slot.StartTime,
                EndTime = se.Slot.EndTime,
                Date = se.Date,
                DayOfWeek = se.DayOfWeek,
                SubjectCode = se.SubjectClass.Subject.SubjectCode,
                ClassName = se.SubjectClass.Class.ClassName
            })
            .ToList();

        return Ok(schedules);
    }
    [HttpGet("GetScheduleByClass/{classId}")]
    public IActionResult GetScheduleByClass(int classId)
    {
        var today = DateTime.UtcNow.Date;

        TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        var vietnamToday = TimeZoneInfo.ConvertTimeFromUtc(today, vietnamTimeZone);

        var startDate = vietnamToday.AddDays(-(int)vietnamToday.DayOfWeek).AddDays(1);

        var endDate = startDate.AddDays(6);

        var schedules = _context.Schedules
            .Include(c => c.SubjectClass)
            .Include(t => t.Teacher)
            .Include(s => s.Slot)
            .Where(se => se.SubjectClass.ClassId == classId && se.Date >= startDate && se.Date <= endDate)
            .Select(se => new GetScheduleDTO
            {
                ScheduleId = se.ScheduleId,
                SubjectClassId = se.SubjectClassId,
                TeacherId = se.TeacherId,
                TeacherName = $"{se.Teacher.FirstName} {se.Teacher.LastName}",
                SlotId = se.SlotId,
                StartTime = se.Slot.StartTime,
                EndTime = se.Slot.EndTime,
                Date = se.Date,
                DayOfWeek = se.DayOfWeek,
                SubjectCode = se.SubjectClass.Subject.SubjectCode,
                ClassName = se.SubjectClass.Class.ClassName
            })
            .ToList();
        return Ok(schedules);

    }
    [HttpGet("SearchScheduleByClass/{classId}/{startDate}/{endDate}/{year}")]
    public IActionResult GetScheduleByClass(int classId, string startDate, string endDate, int year)
    {
        startDate = HttpUtility.UrlDecode(startDate);
        endDate = HttpUtility.UrlDecode(endDate);

        startDate += "/" + year.ToString();
        endDate += "/" + year.ToString();

        var startDateTime = DateTime.ParseExact(startDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        var endDateTime = DateTime.ParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
        var startYear = startDateTime.Year;

        var schedules = _context.Schedules
            .Include(c => c.SubjectClass)
            .Include(t => t.Teacher)
            .Include(s => s.Slot)
            .Where(se => se.SubjectClass.ClassId == classId && se.Date >= startDateTime && se.Date <= endDateTime && (se.Date.HasValue ? se.Date.Value.Year : 0) == year)
            .Select(se => new GetScheduleDTO
            {
                ScheduleId = se.ScheduleId,
                SubjectClassId = se.SubjectClassId,
                TeacherId = se.TeacherId,
                TeacherName = $"{se.Teacher.FirstName} {se.Teacher.LastName}",
                SlotId = se.SlotId,
                StartTime = se.Slot.StartTime,
                EndTime = se.Slot.EndTime,
                Date = se.Date,
                DayOfWeek = se.DayOfWeek,
                SubjectCode = se.SubjectClass.Subject.SubjectCode,
                ClassName = se.SubjectClass.Class.ClassName
            })
            .ToList();

        return Ok(schedules);
    }
    [HttpGet("GetScheduleAll")]
    public IActionResult GetScheduleAll()
    {
        var today = DateTime.UtcNow.Date;

        TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        var vietnamToday = TimeZoneInfo.ConvertTimeFromUtc(today, vietnamTimeZone);

        var startDate = vietnamToday.AddDays(-(int)vietnamToday.DayOfWeek).AddDays(1);

        var endDate = startDate.AddDays(6);

        var schedules = _context.Schedules
            .Include(c => c.SubjectClass)
            .Include(t => t.Teacher)
            .Include(s => s.Slot)
            .Where(se => se.Date >= startDate && se.Date <= endDate)
            .Select(se => new GetScheduleDTO
            {
                ScheduleId = se.ScheduleId,
                SubjectClassId = se.SubjectClassId,
                TeacherId = se.TeacherId,
                TeacherName = $"{se.Teacher.FirstName} {se.Teacher.LastName}",
                SlotId = se.SlotId,
                StartTime = se.Slot.StartTime,
                EndTime = se.Slot.EndTime,
                Date = se.Date,
                DayOfWeek = se.DayOfWeek,
                SubjectCode = se.SubjectClass.Subject.SubjectCode,
                ClassName = se.SubjectClass.Class.ClassName
            })
            .ToList();
        return Ok(schedules);

    }
    [HttpGet("GetScheduleAllByDate/{startDate}/{endDate}/{year}")]
    public IActionResult GetScheduleAllByDate(string startDate, string endDate, int year)
    {
        startDate = HttpUtility.UrlDecode(startDate);
        endDate = HttpUtility.UrlDecode(endDate);

        startDate += "/" + year.ToString();
        endDate += "/" + year.ToString();

        var startDateTime = DateTime.ParseExact(startDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        var endDateTime = DateTime.ParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
        var startYear = startDateTime.Year;

        var schedules = _context.Schedules
            .Include(c => c.SubjectClass)
            .Include(t => t.Teacher)
            .Include(s => s.Slot)
            .Where(se => se.Date >= startDateTime && se.Date <= endDateTime && (se.Date.HasValue ? se.Date.Value.Year : 0) == year)
            .Select(se => new GetScheduleDTO
            {
                ScheduleId = se.ScheduleId,
                SubjectClassId = se.SubjectClassId,
                TeacherId = se.TeacherId,
                TeacherName = $"{se.Teacher.FirstName} {se.Teacher.LastName}",
                SlotId = se.SlotId,
                StartTime = se.Slot.StartTime,
                EndTime = se.Slot.EndTime,
                Date = se.Date,
                DayOfWeek = se.DayOfWeek,
                SubjectCode = se.SubjectClass.Subject.SubjectCode,
                ClassName = se.SubjectClass.Class.ClassName
            })
            .ToList();

        return Ok(schedules);

    }
    [HttpDelete("DeleteSchedule/{id}")]
    public async Task<IActionResult> DeleteSchedule(int id)
    {
        var schedule = await _context.Schedules.FindAsync(id);
        if (schedule == null)
        {
            return NotFound();
        }

        _context.Schedules.Remove(schedule);
        await _context.SaveChangesAsync();

        return NoContent();
    }
    [HttpPut("UpdateSchedule/{id}")]
    public async Task<IActionResult> UpdateSchedule(int id, Schedule updatedSchedule)
    {
        if (id != updatedSchedule.ScheduleId)
        {
            return BadRequest();
        }

        _context.Entry(updatedSchedule).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ScheduleExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    private bool ScheduleExists(int id)
    {
        return _context.Schedules.Any(s => s.ScheduleId == id);
    }
    [HttpGet("ExportScheduleByTeacher/{teacherId}/{startDate}/{endDate}/{year}")]
    public IActionResult ExportScheduleByTeacher(int teacherId, string startDate, string endDate, int year)
    {
        startDate = HttpUtility.UrlDecode(startDate);
        endDate = HttpUtility.UrlDecode(endDate);

        startDate += "/" + year.ToString();
        endDate += "/" + year.ToString();

        var startDateTime = DateTime.ParseExact(startDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        var endDateTime = DateTime.ParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);

        var schedules = _context.Schedules
            .Include(c => c.SubjectClass)
            .Include(t => t.Teacher)
            .Include(s => s.Slot)
            .Where(se => se.TeacherId == teacherId && se.Date >= startDateTime && se.Date <= endDateTime && (se.Date.HasValue ? se.Date.Value.Year : 0) == year)
            .ToList();

        // Sắp xếp schedules theo ngày
        schedules = schedules.OrderBy(se => se.Date).ToList();

        var stream = new MemoryStream();
        using (var package = new ExcelPackage(stream))
        {
            var worksheet = package.Workbook.Worksheets.Add("Schedule");
            worksheet.Cells[1, 1].Value = "Slot ID";

            // Hiển thị các ngày theo tuần
            var currentDate = startDateTime;
            var dayOfWeek = (int)startDateTime.DayOfWeek; // Lấy ngày trong tuần của startDate
            currentDate = currentDate.AddDays(-(dayOfWeek - 1)); // Điều chỉnh ngày bắt đầu để bắt đầu từ thứ Hai

            for (int i = 2; i <= 8; i++)
            {
                worksheet.Cells[1, i].Value = currentDate.ToString("dd/MM/yyyy");
                currentDate = currentDate.AddDays(1);
            }

            foreach (var schedule in schedules)
            {
                if (schedule != null && schedule.Date.HasValue && schedule.SubjectClass != null && schedule.SubjectClass.Class != null)
                {
                    var columnIndex = ((int)schedule.Date.Value.DayOfWeek + 1) % 7 + 2;
                    var rowIndex = schedule.SlotId + 1;

                    if (columnIndex >= 2 && columnIndex <= 8 && rowIndex >= 1)
                    {
                        worksheet.Cells[rowIndex, columnIndex].Value = schedule.SubjectClass.Class.ClassName;
                    }
                    else
                    {
                        Console.WriteLine($"Lỗi: Không thể ghi vào ô [{rowIndex}, {columnIndex}].");

                    }
                }
            }


            package.Save();
        }

        stream.Position = 0;
        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Schedule.xlsx");
    }
    [HttpGet("GetScheduleByTeacherEmail")]
    public IActionResult GetScheduleByTeacherEmail()
    {
        var schedules = _context.Schedules.Include(c => c.SubjectClass)
            .Include(t => t.Teacher)
            .Include(s => s.Slot)
            .Select(se => new GetScheduleDTO
            {
                ScheduleId = se.ScheduleId,
                SubjectClassId = se.SubjectClassId,
                TeacherId = se.TeacherId,
                Email = se.Teacher.Email,
                TeacherName = $"{se.Teacher.FirstName} {se.Teacher.LastName}",
                SlotId = se.SlotId,
                StartTime = se.Slot.StartTime,
                EndTime = se.Slot.EndTime,
                Date = se.Date,
                DayOfWeek = se.DayOfWeek,
                SubjectCode = se.SubjectClass.Subject.SubjectCode,
                ClassName = se.SubjectClass.Class.ClassName
            })
            .ToList();

        return Ok(schedules);
    }
}
    



