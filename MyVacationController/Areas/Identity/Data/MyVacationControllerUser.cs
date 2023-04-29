using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MyVacationController.Areas.Identity.Data;

// Add profile data for application users by adding properties to the MyVacationControllerUser class
public class MyVacationControllerUser : IdentityUser
{
    [PersonalData]
    public string FirstName { get; set; } = string.Empty;

    [PersonalData]
    public string LastName { get; set; } = string.Empty;
}
