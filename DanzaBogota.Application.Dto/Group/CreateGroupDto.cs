namespace DanzaBogota.Application.Dto.Group
{
    public class CreateGroupDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string DirectorName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? SocialMediaUrl { get; set; }
        public int LocalityId { get; set; }
        public int CategoryId { get; set; }
        public bool UseDirectorRut { get; set; }
    }
}
