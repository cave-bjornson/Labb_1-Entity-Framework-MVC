using MyVacationController.Data;

namespace MyVacationController.Models
{
    public class Employee
    {
        public string? Id { get; set; }

        internal ApplicationUser? User { get; set; }

        public string? FirstName => User?.FirstName;
        public string? LastName => User?.LastName;
        public string? Email => User?.Email;
        public DateTime? DOB => User?.DOB;

        public ICollection<Leave> Leaves { get; } = new List<Leave>();
    }
}
