namespace CorporatePassBooking.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int VisitorId { get; set; }
        public int FacilityId { get; set; }
        public DateTime BookingDate { get; set; }
        public string Status { get; set; }
        public Visitor Visitor { get; set; }
        public Facility Facility { get; set; }
    }
}
