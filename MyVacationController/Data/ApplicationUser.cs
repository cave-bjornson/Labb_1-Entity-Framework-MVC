using Microsoft.AspNetCore.Identity;

namespace MyVacationController.Data
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string? FirstName { get; set; }

        [PersonalData]
        public string? LastName { get; set; }

        [PersonalData]
        public DateTime DOB { get; set; }
    }
}
