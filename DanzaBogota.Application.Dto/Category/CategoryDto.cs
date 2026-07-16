namespace DanzaBogota.Application.Dto.Category
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        public string? Description { get; set; }
    }
}
