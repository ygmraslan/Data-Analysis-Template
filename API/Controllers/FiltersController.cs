using DataAnalysis.Application.Common.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataAnalysis.API.Controllers;

[Authorize]
[Route("api/[controller]")]
public class FiltersController : BaseController
{
    [HttpGet("options")]
    public IActionResult GetOptions() => Ok(FilterCatalog.GetOptions());
}