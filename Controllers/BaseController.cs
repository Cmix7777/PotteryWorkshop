using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace PotteryWorkshop.Controllers;

public abstract class BaseController : Controller
{
    protected int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
