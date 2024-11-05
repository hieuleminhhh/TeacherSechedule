using ProjectSchedule.Models;

namespace ProjectSchedule.Dto
{
    public class SubjectDTO
    {
        public int SubjectId { get; set; }
        public string? SubjectName { get; set; }
        public string? SubjectCode { get; set; }
        public ICollection<int> SubjectClassIds { get; set; }
    }
}
