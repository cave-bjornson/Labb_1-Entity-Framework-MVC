﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MyVacationController.Models
{
    public class LeaveViewModel : IValidatableObject
    {
        public LeaveViewModel() { }

        public static IEnumerable<SelectListItem> LeaveTypes { get; } =
            Enum.GetValues<LeaveType>()
                .Select(lt => new SelectListItem(lt.ToFriendlyString(), lt.ToString()));

        public required Guid? Id { get; init; }

        public Guid EmployeeId { get; init; }

        public string? EmployeeFirstName { get; init; }

        public string? EmployeeLastName { get; init; }

        [DisplayName("Employee")]
        public string? EmployeeFullName => $"{EmployeeFirstName} {EmployeeLastName}";

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public required DateOnly Start { get; init; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public required DateOnly End { get; init; }

        public required int TotalDays { get; init; }

        public required LeaveType Type { get; init; }

        [StringLength(500)]
        public string? Comment { get; init; }

        public string? ShortComment => Comment?.Length >= 20 ? $"{Comment[..16]}..." : Comment;

        [DataType(DataType.Date)]
        public required DateTime Created { get; init; }

        /// <inheritdoc />
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Start > End)
            {
                yield return new ValidationResult("Start date must be before end date.");
            }

            if (Start < DateOnly.FromDateTime(DateTime.Today))
            {
                yield return new ValidationResult("Start date must be in the future.");
            }

            if (End < DateOnly.FromDateTime(DateTime.Today))
            {
                yield return new ValidationResult("End date must be in the future.");
            }

            if (End < Start)
            {
                yield return new ValidationResult("End date must be after start date.");
            }
        }
    }
}
