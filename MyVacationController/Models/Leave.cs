﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MyVacationController.Models
{
    public class Leave
    {
        public Guid? Id { get; set; }
        public required DateOnly Start { get; set; }
        public required DateOnly End { get; set; }

        internal Guid? EmployeeId { get; set; }
        public required Employee Employee { get; set; }

        public required LeaveType Type { get; set; }
        public string? Comment { get; set; }
        public DateTime Created { get; set; }

        public int TotalDays =>
            End.ToDateTime(TimeOnly.MinValue).Subtract(Start.ToDateTime(TimeOnly.MinValue)).Days;
    }

    public enum LeaveType
    {
        Vacation,
        LeaveOfAbsence,
        Parental,
        Sabbatical
    }

    public static class LeaveTypeExtensions
    {
        public static string ToFriendlyString(this LeaveType type)
        {
            return type switch
            {
                LeaveType.Vacation => "Vacation",
                LeaveType.LeaveOfAbsence => "Leave of Absence",
                LeaveType.Parental => "Parental Leave",
                LeaveType.Sabbatical => "Sabbatical Leave",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}
