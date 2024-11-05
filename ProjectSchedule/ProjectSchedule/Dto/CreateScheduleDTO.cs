namespace ProjectSchedule.Dto
{
    public class CreateScheduleDTO
    {
        public int ScheduleId { get; set; }
        public int SubjectClassId { get; set; }
        public int TeacherId { get; set; }
        public int SlotId { get; set; }
        public DateTime? Date { get; set; }
        public string? DayOfWeek { get; set; }
        
    }
}
