namespace CorporatePassBooking.Models
{
    public class Facility
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Capacity { get; set; }
        public string Location { get; set; }
        public string? Amenities { get; set; }


        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
