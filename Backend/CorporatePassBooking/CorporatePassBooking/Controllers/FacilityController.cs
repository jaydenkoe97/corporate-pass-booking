using CorporatePassBooking.Data;
using CorporatePassBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CorporatePassBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacilityController : ControllerBase
    {
        private readonly ZooDbContext _context;

        public FacilityController(ZooDbContext context)
        {
            _context = context;
        }

        [HttpGet("FacilityList")]
        public async Task<ActionResult<IEnumerable<Facility>>> GetFacilities()
        {
            return await _context.Facilities.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Facility>> GetFacilityById(int id)
        {
            return await _context.Facilities.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

    }
}
