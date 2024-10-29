using CorporatePassBooking.Controllers;
using CorporatePassBooking.Data;
using CorporatePassBooking.DTOs;
using CorporatePassBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CorporatePassBooking.UnitTest
{
    public class BookingControllerTests
    {
        private readonly BookingController _controller;
        private readonly ZooDbContext _context;

        public BookingControllerTests()
        {
            var options = new DbContextOptionsBuilder<ZooDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ZooDbContext(options);
            _controller = new BookingController(_context);
        }

        [Fact]
        public async Task CreateBooking_ReturnsBadRequest_WhenBookingIsNull()
        {
            // Act
            var result = await _controller.CreateBooking(null);

            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Booking input parameter is null.", actionResult.Value);
        }

        [Fact]
        public async Task CreateBooking_ReturnsBadRequest_WhenBookingDateIsInThePast()
        {
            // Arrange
            var booking = new BookingDto { VisitorId = 1, FacilityId = 1, BookingDate = DateTime.Now.AddDays(-1), Status = "Booked" };

            // Act
            var result = await _controller.CreateBooking(booking);

            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Booking date cannot be in the past.", actionResult.Value);
        }

        [Fact]
        public async Task CreateBooking_ReturnsNotFound_WhenFacilityDoesNotExist()
        {
            // Arrange
            var booking = new BookingDto { FacilityId = 1, BookingDate = DateTime.Now.AddDays(1) };

            // Act
            var result = await _controller.CreateBooking(booking);

            // Assert
            var actionResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("The specified facility does not exist.", actionResult.Value);
        }

        [Fact]
        public async Task CreateBooking_ReturnsConflict_WhenDoubleBookingExists()
        {
            // Arrange
            var facility = new Facility { Id = 1, Name = "Test Facility", Type = "Test", Capacity = 100, Location = "Zoo Park" };
            var existingBooking = new Booking { Id = 1, VisitorId = 1, FacilityId = 1, BookingDate = DateTime.Now.AddDays(1), Status = "Confirmed" };
            var vistor = new Visitor { Id = 1, Name = "John", Email = "johntest@gmail.com", PhoneNumber = "010-1111111" };

            _context.Facilities.Add(facility);
            _context.Bookings.Add(existingBooking);
            _context.Visitors.Add(vistor);
            await _context.SaveChangesAsync();

            var newBooking = new BookingDto { VisitorId = 1, FacilityId = 1, BookingDate = existingBooking.BookingDate, Status = "Confirmed", VisitorName = "John", FacilityName = "Test Facility" };

            // Act
            var result = await _controller.CreateBooking(newBooking);

            // Assert
            var actionResult = Assert.IsType<ConflictObjectResult>(result.Result);
            Assert.Equal("A booking already exists for this facility and visitor at the selected date.", actionResult.Value);
        }

        [Fact]
        public async Task CreateBooking_ReturnsCreated_WhenBookingIsSuccessful()
        {
            // Arrange
            var booking = new BookingDto { VisitorId = 1, FacilityId = 1, BookingDate = DateTime.Now.AddDays(1), Status = "Confirmed", VisitorName = "John", FacilityName = "Test Facility" };

            // Act
            var result = await _controller.CreateBooking(booking);

            // Assert
            var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal("GetBookingById", actionResult.ActionName);
            Assert.Equal(booking, actionResult.Value);
        }
    }
}
