using Asp.Versioning;
using CitiesManager.Infrastructure.DatabaseContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CitiesManager.WebAPI.Controllers.v2
{
    [ApiVersion("2.0")]
    public class CitiesController : CustomControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Cities
        /// <summary>
        /// To get list of cities(only city name) from the database.
        /// </summary>
        /// <returns>List of cities, ordered by city name</returns>
        [HttpGet]
        //[Produces("application/json")]
        public async Task<ActionResult<IEnumerable<string?>>> GetCities()
        {
            return await _context.Cities.OrderBy(c => c.CityName).Select(c => c.CityName).ToListAsync();
        }

    }
}
