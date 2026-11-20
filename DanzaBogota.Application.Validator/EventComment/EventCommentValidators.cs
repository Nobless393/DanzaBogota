using DanzaBogota.Application.Dto.EventComment;
using FluentValidation;

namespace DanzaBogota.Application.Validator.EventComment
{
    public class CreateEventCommentDtoValidator : AbstractValidator<CreateEventCommentDto>
    {
        public CreateEventCommentDtoValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("El comentario es obligatorio.")
                .MaximumLength(500).WithMessage("El comentario no puede superar 500 caracteres.");

            RuleFor(x => x.ParentId)
                .GreaterThan(0).When(x => x.ParentId.HasValue)
                .WithMessage("Comentario padre inválido.");
        }
    }
}
