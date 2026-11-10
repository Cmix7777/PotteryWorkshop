using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace PotteryWorkshop.Controllers;

/// <summary>Общий предок контроллеров: даёт Id вошедшего пользователя из cookie.</summary>
public abstract class BaseController : Controller
{
    protected int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
