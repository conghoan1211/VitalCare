using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected string GetUserId()
        {
            return User.FindFirst("UserID")?.Value;
        }
    }
}
