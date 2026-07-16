namespace DanzaBogota.Application.Dto.Event
{
    public class CreateEventDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string LocationName { get; set; }
        public string Address { get; set; }
        public int LocalityId { get; set; }
        public int MaxParticipants { get; set; }
        public int MaxParticipantsPerGroup { get; set; }
        public List<int> EventTypeIds { get; set; }
        public List<int> CategoryIds { get; set; }
    }
}
