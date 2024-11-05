using System;
using System.Collections.Generic;

namespace ProjectSchedule.Models
{
    public partial class Semester
    {
        public Semester()
        {
            Subjects = new HashSet<Subject>();
        }

        public int SemesterId { get; set; }
        public string? SemesterName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public virtual ICollection<Subject> Subjects { get; set; }
    }
}
