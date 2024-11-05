using Microsoft.AspNetCore.Mvc;

namespace ProjectSchedule.Dto
{
    public partial class Register
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }

    }
}
