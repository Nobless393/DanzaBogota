namespace DanzaBogota.Application.Dto.Group
{
    public class GroupDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string DirectorName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? SocialMediaUrl { get; set; }
        public string? RutUrl { get; set; }
        public bool UseDirectorRut { get; set; }
        public int LocalityId { get; set; }
        public string LocalityName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public bool IsActive { get; set; }
        public string DirectorFullName { get; set; }
    }
}
