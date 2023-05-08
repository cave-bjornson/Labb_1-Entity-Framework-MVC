using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyVacationController.Models;

public record MonthViewModel([DataType("month")] DateTime Month);
