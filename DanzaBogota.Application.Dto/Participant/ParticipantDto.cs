using DanzaBogota.Domain.Entity.Enums;

namespace DanzaBogota.Application.Dto.Participant
{
    public class ParticipantDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public DocumentType DocumentType { get; set; }
        public string DocumentNumber { get; set; }
        public int Age { get; set; }
        public DateOnly BirthDate { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public bool IsActive { get; set; }
    }
}
