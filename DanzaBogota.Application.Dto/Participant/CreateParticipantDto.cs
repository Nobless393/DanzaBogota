using DanzaBogota.Domain.Entity.Enums;

namespace DanzaBogota.Application.Dto.Participant
{
    public class CreateParticipantDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DocumentType DocumentType { get; set; }
        public string DocumentNumber { get; set; }
        public DateOnly BirthDate { get; set; }
        public int GroupId { get; set; }
    }
}
