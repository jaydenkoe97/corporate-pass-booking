namespace CorporatePassBooking.DTOs
{
    public class BookingDto
    {
        public int Id { get; set; }
        public int VisitorId { get; set; }
        public int FacilityId { get; set; }
        public string VisitorName { get; set; }
        public string FacilityName { get; set; }
        public DateTime BookingDate { get; set; }
        public string Status { get; set; }
    }
}
