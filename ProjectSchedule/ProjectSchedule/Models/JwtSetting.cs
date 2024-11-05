namespace ProjectSchedule.Models
{
    public class JwtSetting
    {
        public string? Secret { get; set; }
        public int ExpiryHours { get; set; }
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
    }
}
