using Microsoft.AspNetCore.Mvc;
using ProjectSchedule.Models;

namespace ProjectSchedule.Dto
{
    public class ScheduleViewDTO
    {
        public int ClassId { get; set; }
        public string? ClassName { get; set; }
        public int TeacherId { get; set; }
        public string? TeacherName { get; set; }
        public int SlotId { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string? SubjectCode { get; set; }
        public string? DayOfWeek { get; set; }
    }
}
