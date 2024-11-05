using System;
using System.Collections.Generic;

namespace ProjectSchedule.Models
{
    public partial class Class
    {
        public Class()
        {
            SubjectClasses = new HashSet<SubjectClass>();
        }

        public int ClassId { get; set; }
        public string? ClassName { get; set; }

        public virtual ICollection<SubjectClass> SubjectClasses { get; set; }
    }
}
