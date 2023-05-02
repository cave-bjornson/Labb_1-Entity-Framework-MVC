namespace MyVacationController.Models
{
    public class Leave
    {
        public string? Id { get; set; }
        public required DateTime Start { get; set; }
        public required DateTime End { get; set; }
        public required Employee Employee { get; set; }

        public required LeaveType Type { get; set; }

        public string? Comment { get; set; }
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
