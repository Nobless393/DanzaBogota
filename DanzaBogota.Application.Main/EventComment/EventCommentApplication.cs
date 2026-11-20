using DanzaBogota.Application.Dto.EventComment;
using DanzaBogota.Application.Interface;
using DanzaBogota.Domain.Entity.Entities;
using DanzaBogota.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DanzaBogota.Application.Main.EventComment
{
    public class EventCommentApplication : IEventCommentApplication
    {
        private readonly ApplicationDbContext _context;

        public EventCommentApplication(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EventCommentDto>> GetByEventAsync(int eventId, string sort, int userId, bool isAdminOrGestor)
        {
            var comments = await _context.EventComments
                .Include(c => c.User)
                .Include(c => c.Likes)
                .Include(c => c.Replies)
                    .ThenInclude(r => r.User)
                .Include(c => c.Replies)
                    .ThenInclude(r => r.Likes)
                .Where(c => c.EventId == eventId && c.ParentId == null)
                .ToListAsync();

            var commentIds = comments
                .SelectMany(c => new[] { c.Id }.Concat(c.Replies.Select(r => r.Id)))
                .ToList();

            var likedIds = new HashSet<int>();

            if (userId > 0 && commentIds.Count > 0)
            {
                var liked = await _context.EventCommentLikes
                    .Where(l => commentIds.Contains(l.CommentId) && l.UserId == userId)
                    .Select(l => l.CommentId)
                    .ToListAsync();

                likedIds = liked.ToHashSet();
            }

            var eventOwnerId = await _context.Events
                .Where(e => e.Id == eventId)
                .Select(e => e.CreatedByUserId)
                .FirstOrDefaultAsync();

            var ordered = sort == "liked"
                ? comments.OrderByDescending(c => c.Likes.Count).ThenByDescending(c => c.Created)
                : comments.OrderByDescending(c => c.Created);

            return ordered
                .Select(c => Map(c, userId, eventOwnerId, isAdminOrGestor, likedIds))
                .ToList();
        }

        public async Task<(bool Success, string Message, EventCommentDto? Comment)> CreateAsync(
            int eventId, int userId, CreateEventCommentDto dto)
        {
            var eventExists = await _context.Events
                .AnyAsync(e => e.Id == eventId && e.IsActive);

            if (!eventExists)
                return (false, "Evento no encontrado.", null);

            if (dto.ParentId.HasValue)
            {
                var parentValid = await _context.EventComments
                    .AnyAsync(c => c.Id == dto.ParentId.Value
                        && c.EventId == eventId
                        && c.ParentId == null
                        && !c.IsDeleted);

                if (!parentValid)
                    return (false, "El comentario al que intentas responder no es válido.", null);
            }

            var comment = new Domain.Entity.Entities.EventComment
            {
                Content = dto.Content.Trim(),
                EventId = eventId,
                UserId = userId,
                ParentId = dto.ParentId,
                IsDeleted = false,
                Created = DateTimeOffset.UtcNow,
                CreatedBy = userId.ToString()
            };

            _context.EventComments.Add(comment);
            await _context.SaveChangesAsync();

            var created = await _context.EventComments
                .Include(c => c.User)
                .FirstAsync(c => c.Id == comment.Id);

            var dtoResult = Map(created, userId, eventOwnerId: 0, isAdminOrGestor: false, new HashSet<int>());
            return (true, "Comentario publicado.", dtoResult);
        }

        public async Task<(bool Success, string Message)> DeleteAsync(
            int eventId, int commentId, int userId, bool isAdminOrGestor)
        {
            var comment = await _context.EventComments
                .Include(c => c.Event)
                .Include(c => c.Replies)
                .FirstOrDefaultAsync(c => c.Id == commentId && c.EventId == eventId);

            if (comment == null)
                return (false, "Comentario no encontrado.");

            var isOwner = comment.UserId == userId;
            var isEventOwner = comment.Event.CreatedByUserId == userId;

            if (!isOwner && !isEventOwner && !isAdminOrGestor)
                return (false, "No tienes permiso para eliminar este comentario.");

            comment.IsDeleted = true;
            comment.LastModified = DateTimeOffset.UtcNow;
            comment.LastModifiedBy = userId.ToString();

            foreach (var reply in comment.Replies)
            {
                reply.IsDeleted = true;
                reply.LastModified = DateTimeOffset.UtcNow;
                reply.LastModifiedBy = userId.ToString();
            }

            await _context.SaveChangesAsync();

            return (true, "Comentario eliminado.");
        }

        public async Task<(bool Success, string Message, int LikesCount, bool Liked)> ToggleLikeAsync(int commentId, int userId)
        {
            var comment = await _context.EventComments
                .FirstOrDefaultAsync(c => c.Id == commentId && !c.IsDeleted);

            if (comment == null)
                return (false, "Comentario no encontrado.", 0, false);

            var existing = await _context.EventCommentLikes
                .FirstOrDefaultAsync(l => l.CommentId == commentId && l.UserId == userId);

            if (existing != null)
            {
                _context.EventCommentLikes.Remove(existing);
                await _context.SaveChangesAsync();

                var count = await _context.EventCommentLikes
                    .CountAsync(l => l.CommentId == commentId);

                return (true, "Like retirado.", count, false);
            }

            _context.EventCommentLikes.Add(new EventCommentLike
            {
                CommentId = commentId,
                UserId = userId
            });

            await _context.SaveChangesAsync();

            var newCount = await _context.EventCommentLikes
                .CountAsync(l => l.CommentId == commentId);

            return (true, "Te gusta este comentario.", newCount, true);
        }

        private static EventCommentDto Map(
            Domain.Entity.Entities.EventComment comment,
            int currentUserId,
            int eventOwnerId,
            bool isAdminOrGestor,
            HashSet<int> likedIds)
        {
            var dto = new EventCommentDto
            {
                Id = comment.Id,
                Content = comment.IsDeleted ? string.Empty : comment.Content,
                IsDeleted = comment.IsDeleted,
                ParentId = comment.ParentId,
                EventId = comment.EventId,
                UserId = comment.UserId,
                AuthorFirstName = comment.User?.FirstName ?? string.Empty,
                AuthorLastName = comment.User?.LastName ?? string.Empty,
                CreatedAt = comment.Created,
                LikesCount = comment.Likes?.Count ?? 0,
                LikedByCurrentUser = likedIds.Contains(comment.Id),
                CanModerate = comment.UserId == currentUserId,
                Replies = new List<EventCommentDto>()
            };

            if (comment.Replies != null)
            {
                dto.Replies = comment.Replies
                    .OrderBy(r => r.Created)
                    .Select(r => Map(r, currentUserId, eventOwnerId, isAdminOrGestor, likedIds))
                    .ToList();
            }

            return dto;
        }
    }
}
