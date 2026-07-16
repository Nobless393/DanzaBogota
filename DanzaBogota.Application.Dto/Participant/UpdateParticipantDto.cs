using DanzaBogota.Domain.Entity.Enums;

namespace DanzaBogota.Application.Dto.Participant
{
    public class UpdateParticipantDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DocumentType DocumentType { get; set; }
        public string DocumentNumber { get; set; }
        public DateOnly BirthDate { get; set; }
    }
}
