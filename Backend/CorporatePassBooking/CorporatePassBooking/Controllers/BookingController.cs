using CorporatePassBooking.Data;
using CorporatePassBooking.DTOs;
using CorporatePassBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CorporatePassBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly ZooDbContext _context;

        public BookingController(ZooDbContext context)
        {
            _context = context;
        }

        [HttpGet("BookingList")]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookings()
        {
            return await _context.Bookings
                .Join(_context.Visitors,
                    booking => booking.VisitorId,
                    visitor => visitor.Id,
                    (booking, visitor) => new { booking, visitor })
                .Join(_context.Facilities,
                    ec => ec.booking.FacilityId,
                    facility => facility.Id,
                    (ec, facility) => new BookingDto
                    {
                        Id = ec.booking.Id,
                        FacilityId = facility.Id,
                        VisitorId = ec.visitor.Id,
                        FacilityName = facility.Name,
                        VisitorName = ec.visitor.Name,
                        BookingDate = ec.booking.BookingDate,
                        Status = ec.booking.Status
                    }).ToListAsync();
        }

        [HttpGet("BookingList/{visitorId}")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookingsByVisitorId(int visitorId)
        {
            Visitor? existingVisitor = await _context.Visitors.Where(v => v.Id == visitorId).FirstOrDefaultAsync();

            if (existingVisitor == null)
            {
                return NotFound("The specified vistor does not exist.");
            }

            return await _context.Bookings.Where(x => x.VisitorId == visitorId).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBookingById(int id)
        {
            return await _context.Bookings.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        [HttpPost]
        public async Task<ActionResult<BookingDto>> CreateBooking(BookingDto booking)
        {
            if (booking == null)
            {
                return BadRequest("Booking input parameter is null.");
            }

            if (booking.BookingDate < DateTime.Now)
            {
                return BadRequest("Booking date cannot be in the past.");
            }

            Facility? existingFacility = await _context.Facilities.Where(f => f.Name == booking.FacilityName).FirstOrDefaultAsync();
            if (existingFacility == null)
            {
                return NotFound("The specified facility does not exist.");
            }
            else if (existingFacility.Capacity <= 0)
            {
                return BadRequest("There is no available slot for this facility.");
            }

            Visitor? existingVisitor = await _context.Visitors.Where(f => f.Name == booking.VisitorName).FirstOrDefaultAsync();
            if (existingVisitor == null)
            {
                return NotFound("The specified visitor does not exist.");
            }

            Booking? existingBooking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.FacilityId == existingFacility.Id
                                        && b.VisitorId == existingVisitor.Id
                                        && b.BookingDate == booking.BookingDate
                                        && b.Status != "Cancelled");

            if (existingBooking != null)
            {
                return Conflict("A booking already exists for this facility and visitor at the selected date.");
            }

            Booking newBooking = new Booking();
            newBooking.FacilityId = existingFacility.Id;
            newBooking.VisitorId = existingVisitor.Id;
            newBooking.BookingDate = booking.BookingDate;
            newBooking.Status = booking.Status;

            existingFacility.Capacity -= 1;

            _context.Bookings.Add(newBooking);
            _context.Update(existingFacility);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBookingById), new { id = booking.Id }, booking);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditBooking(int id, BookingDto booking)
        {
            if (booking == null)
            {
                return NotFound("Booking input paramater is null.");
            }

            if (booking.BookingDate < DateTime.Now)
            {
                return BadRequest("Booking date cannot be in the past.");
            }

            Facility? existingFacility = await _context.Facilities.Where(f => f.Name == booking.FacilityName).FirstOrDefaultAsync();
            if (existingFacility == null)
            {
                return NotFound("The specified facility does not exist.");
            }
            else if (existingFacility.Capacity <= 0)
            {
                return BadRequest("There is no available slot for this facility.");
            }

            Visitor? existingVisitor = await _context.Visitors.Where(f => f.Name == booking.VisitorName).FirstOrDefaultAsync();
            if (existingVisitor == null)
            {
                return NotFound("The specified visitor does not exist.");
            }

            Booking? otherBooking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.FacilityId == existingFacility.Id
                                        && b.VisitorId == existingVisitor.Id
                                        && b.BookingDate == booking.BookingDate
                                        && b.Id != id
                                        && b.Status != "Cancelled");

            if (otherBooking != null)
            {
                return Conflict("Another booking already exists for this facility and visitor at the selected date.");
            }

            Booking? existingBooking = await _context.Bookings.Where(b => b.Id == id).FirstOrDefaultAsync();

            if (existingBooking != null)
            {
                Facility? currentFacility = await _context.Facilities.Where(f => f.Id == existingBooking.FacilityId).FirstOrDefaultAsync();
                Facility? newFacility = await _context.Facilities.Where(f => f.Id == existingFacility.Id).FirstOrDefaultAsync();

                existingBooking.VisitorId = existingVisitor.Id;
                existingBooking.FacilityId = existingFacility.Id;
                existingBooking.BookingDate = booking.BookingDate;
                existingBooking.Status = booking.Status;

                currentFacility.Capacity += 1;
                newFacility.Capacity -= 1;

                _context.Update(existingBooking);
                _context.Update(currentFacility);
                _context.Update(newFacility);
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }
    }
}