namespace DanzaBogota.Application.Dto.EventComment
{
    public class CreateEventCommentDto
    {
        public string Content { get; set; }
        public int? ParentId { get; set; }
    }
}
