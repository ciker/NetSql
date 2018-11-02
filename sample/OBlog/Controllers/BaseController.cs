using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OBlog.Infrastructure.FluentValidationExtensions;

namespace OBlog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ValidateModelFilter]
    public class BaseController : ControllerBase
    {
    }
}