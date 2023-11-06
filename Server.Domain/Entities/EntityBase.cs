namespace Server.Domain.Entities
{
    public abstract class EntityBase
    {
        public string HotelName { get; set; }
        public string? City { get; set;}
        public string? Latitude { get; set;}
        public string? Longitude { get; set; }
    }
}
