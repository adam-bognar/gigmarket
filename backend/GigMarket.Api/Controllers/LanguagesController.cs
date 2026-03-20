using GigMarket.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GigMarket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LanguagesController(IApplicationDbContext db) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var languages = await db.Languages
                .ToListAsync(ct);
            return Ok(languages);
        }
    }
}
