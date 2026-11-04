namespace DanzaBogota.Application.Dto.Inscription
{
    public class InscriptionDto
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string CategoryName { get; set; }
        public int EventId { get; set; }
        public string EventTitle { get; set; }
        public DateTimeOffset AppliedAt { get; set; }
        public string Status { get; set; }
        public string? Observations { get; set; }
        public int ParticipantCount { get; set; }
    }
}
