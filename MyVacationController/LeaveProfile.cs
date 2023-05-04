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
                configuration => configuration.MapFrom(leave => leave.Employee.User.FirstName)
            )
            .ForMember(
                model => model.EmployeeLastName,
                configuration => configuration.MapFrom(leave => leave.Employee.User.LastName)
            );
        
        CreateMap<LeaveViewModel, Leave>()
            .ForMember(leave => leave.Employee, opt => opt.MapAtRuntime());
    }
}
