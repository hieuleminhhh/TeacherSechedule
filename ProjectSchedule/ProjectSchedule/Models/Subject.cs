using System;
using System.Collections.Generic;

namespace ProjectSchedule.Models
{
    public partial class Subject
    {
        public Subject()
        {
            SubjectClasses = new HashSet<SubjectClass>();
        }

        public int SubjectId { get; set; }
        public string? SubjectName { get; set; }
        public string? SubjectCode { get; set; }
        public int? SemesterId { get; set; }

        public virtual Semester? Semester { get; set; }
        public virtual ICollection<SubjectClass> SubjectClasses { get; set; }
    }
}
