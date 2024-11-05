using Microsoft.AspNetCore.Mvc;

namespace ProjectSchedule.Dto
{
    public class CreateSubjectDTO 
    {
        public string? SubjectName { get; set; }
        public string? SubjectCode { get; set; }
        public int? SemesterId { get; set; }
    }
}
