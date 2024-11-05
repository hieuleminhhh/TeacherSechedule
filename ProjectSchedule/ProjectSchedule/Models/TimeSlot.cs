using System;
using System.Collections.Generic;

namespace ProjectSchedule.Models
{
    public partial class TimeSlot
    {
        public TimeSlot()
        {
            Schedules = new HashSet<Schedule>();
        }

        public int SlotId { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }

        public virtual ICollection<Schedule> Schedules { get; set; }
    }
}
