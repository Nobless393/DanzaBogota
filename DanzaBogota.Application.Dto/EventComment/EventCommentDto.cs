namespace DanzaBogota.Application.Dto.EventComment
{
    public class EventCommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int? ParentId { get; set; }
        public bool IsDeleted { get; set; }
        public int EventId { get; set; }
        public int UserId { get; set; }
        public string AuthorFirstName { get; set; }
        public string AuthorLastName { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public int LikesCount { get; set; }
        public bool LikedByCurrentUser { get; set; }
        public bool CanModerate { get; set; }
        public List<EventCommentDto> Replies { get; set; }
    }
}
