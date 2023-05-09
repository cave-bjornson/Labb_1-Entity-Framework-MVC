using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyVacationController.Models;

public class EmployeeViewModel
{
    public Guid Id { get; init; }

    [DisplayName("Employee")]
    public string? EmployeeFullName => $"{EmployeeFirstName} {EmployeeLastName}";

    public string? EmployeeFirstName { get; init; }
    public string? EmployeeLastName { get; init; }

    [DataType(DataType.EmailAddress)]
    public string? Email { get; init; }

    [DisplayName("Applied for Leave?")]
    public bool HasApplications { get; init; }

    [DisplayName("Date of Birth")]
    public DateOnly? DOB { get; init; }

    [DisplayName("Total Days")]
    public int TotalDaysThisYear { get; init; }
};
