using AutoMapper;
using MyVacationController.Models;

namespace MyVacationController;

public class LeaveProfile : Profile
{
    public LeaveProfile()
    {
        CreateProjection<Leave, LeaveViewModel>()
            .ForMember(
                model => model.EmployeeFirstName,
                configuration => configuration.MapFrom(leave => leave.Employee.User.GivenName)
            )
            .ForMember(
                model => model.EmployeeLastName,
                configuration => configuration.MapFrom(leave => leave.Employee.User.SurName)
            )
            .ForMember(
                model => model.EmployeeId,
                configuration => configuration.MapFrom(leave => leave.Employee.Id)
            );

        CreateMap<LeaveViewModel, Leave>()
            .ForMember(leave => leave.Employee, opt => opt.MapAtRuntime());

        CreateMap<Employee, EmployeeViewModel>()
            .ForMember(
                model => model.EmployeeFirstName,
                configuration => configuration.MapFrom(employee => employee.FirstName)
            )
            .ForMember(
                model => model.EmployeeLastName,
                configuration => configuration.MapFrom(employee => employee.LastName)
            )
            .ForMember(
                model => model.HasApplications,
                configuration => configuration.MapFrom(employee => employee.Leaves.Any())
            )
            .ForMember(
                model => model.TotalDaysThisYear,
                configuration =>
                {
                    configuration.MapFrom(
                        employee =>
                            employee.Leaves
                                .ToList()
                                .Aggregate(0, (total, leave) => total + leave.TotalDays)
                    );
                }
            );
    }
}
