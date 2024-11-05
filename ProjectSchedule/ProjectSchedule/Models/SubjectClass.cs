using System;
using System.Collections.Generic;

namespace ProjectSchedule.Models
{
    public partial class SubjectClass
    {
        public SubjectClass()
        {
            Schedules = new HashSet<Schedule>();
        }

        public int SubjectClassId { get; set; }
        public int? SubjectId { get; set; }
        public int? ClassId { get; set; }

        public virtual Class? Class { get; set; }
        public virtual Subject? Subject { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; }
    }
}
