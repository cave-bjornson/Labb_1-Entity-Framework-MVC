using Microsoft.AspNetCore.Identity;

namespace MyVacationController.Data
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public Guid? EmployeeId { get; set; }

        [PersonalData]
        public string? GivenName { get; set; }

        [PersonalData]
        public string? SurName { get; set; }

        [PersonalData]
        public DateOnly? DateOfBirth { get; set; }

        public bool IsAdmin { get; set; }
    }
}
