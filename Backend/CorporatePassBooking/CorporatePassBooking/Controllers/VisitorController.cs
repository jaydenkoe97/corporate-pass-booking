using CorporatePassBooking.Data;
using CorporatePassBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CorporatePassBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VisitorController : ControllerBase
    {
        private readonly ZooDbContext _context;

        public VisitorController(ZooDbContext context)
        {
            _context = context;
        }

        [HttpGet("VisitorList")]
        public async Task<ActionResult<IEnumerable<Visitor>>> GetVisitors()
        {
            return await _context.Visitors.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Visitor>> GetVisitorById(int id)
        {
            return await _context.Visitors.Where(x => x.Id == id).FirstOrDefaultAsync();
        }
        
        [HttpPost]
        public async Task<ActionResult<Visitor>> CreateVisitor(Visitor visitor)
        {
            if (visitor == null)
            {
                return NotFound("Vistor input paramater is null.");
            }

            Visitor? existingVisitor = await _context.Visitors.Where(x => x.Name == visitor.Name).FirstOrDefaultAsync();

            if (existingVisitor == null)
            {
                _context.Visitors.Add(visitor);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetVisitorById), new { id = visitor.Id }, visitor);
            }

            return Conflict("A visitor already exists with this visitor name.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditVisitor(int id, Visitor visitor)
        {
            if (visitor == null)
            {
                return NotFound("Vistor input paramater is null.");
            }

            var existingVisitor = await _context.Visitors.AsNoTracking().Where(x => x.Id == id).FirstOrDefaultAsync();
            if (existingVisitor == null)
            {
                return NotFound("No existing visitor has been found.");
            }

            existingVisitor.Name = visitor.Name;
            existingVisitor.Email = visitor.Email;
            existingVisitor.PhoneNumber = visitor.PhoneNumber;

            _context.Update(existingVisitor);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
