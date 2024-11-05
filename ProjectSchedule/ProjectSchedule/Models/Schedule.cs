using System;
using System.Collections.Generic;

namespace ProjectSchedule.Models
{
    public partial class Schedule
    {
        public int ScheduleId { get; set; }
        public int SubjectClassId { get; set; }
        public int TeacherId { get; set; }
        public int SlotId { get; set; }
        public DateTime? Date { get; set; }
        public string? DayOfWeek { get; set; }

        public virtual TimeSlot Slot { get; set; } = null!;
        public virtual SubjectClass SubjectClass { get; set; } = null!;
        public virtual Teacher Teacher { get; set; } = null!;
    }
}
