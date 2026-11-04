namespace DanzaBogota.Application.Dto.Event
{
    public class EventDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string LocationName { get; set; }
        public string Address { get; set; }
        public int LocalityId { get; set; }
        public string LocalityName { get; set; }
        public int MaxParticipants { get; set; }
        public int MaxParticipantsPerGroup { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
        public string CreatedByName { get; set; }
        public List<string> EventTypes { get; set; }
        public List<string> Categories { get; set; }
    }
}
