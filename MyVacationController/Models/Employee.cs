using Microsoft.AspNetCore.SignalR;
using MyVacationController.Data;

namespace MyVacationController.Models
{
    public class Employee
    {
        public Guid? Id { get; set; }

        internal Guid? UserId { get; set; }
        internal ApplicationUser? User { get; set; }

        public string? FirstName => User?.GivenName;
        public string? LastName => User?.SurName;

        public string? FullName => $"{FirstName} {LastName}";
        public string? Email => User?.Email;
        public DateOnly? DOB => User?.DateOfBirth;

        public ICollection<Leave> Leaves { get; } = new List<Leave>();
    }
}
