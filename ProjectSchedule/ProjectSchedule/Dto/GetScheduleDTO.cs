using Microsoft.AspNetCore.Mvc;
using ProjectSchedule.Models;

namespace ProjectSchedule.Dto
{
    public class GetScheduleDTO
    {
        public int ScheduleId { get; set; }
        public int SubjectClassId { get; set; }
        public int TeacherId { get; set; }
        public string? TeacherName { get; set; }
        public string? Email { get; set; }
        public int SlotId { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public DateTime? Date { get; set; }
        public string? DayOfWeek { get; set; }
        public string? SubjectCode { get; set; }
        public string? ClassName { get; set; }
    }
}
