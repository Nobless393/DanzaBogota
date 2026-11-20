using DanzaBogota.Application.Dto.Inscription;
using DanzaBogota.Application.Dto.User;
using FluentValidation;

namespace DanzaBogota.Application.Validator.Inscription
{
    public class CreateInscriptionDtoValidator : AbstractValidator<CreateInscriptionDto>
    {
        public CreateInscriptionDtoValidator()
        {
            RuleFor(x => x.GroupId)
                .GreaterThan(0).WithMessage("Debes seleccionar un grupo.");

            RuleFor(x => x.EventId)
                .GreaterThan(0).WithMessage("Debes seleccionar un evento.");
        }
    }

    public class ReviewInscriptionDtoValidator : AbstractValidator<ReviewInscriptionDto>
    {
        public ReviewInscriptionDtoValidator()
        {
            RuleFor(x => x.InscriptionId)
                .GreaterThan(0).WithMessage("Id de inscripción inválido.");

            RuleFor(x => x.Observations)
                .MaximumLength(500).WithMessage("Las observaciones no pueden superar 500 caracteres.");
        }
    }

    public class UpdateInscriptionParticipantsDtoValidator : AbstractValidator<UpdateInscriptionParticipantsDto>
    {
        public UpdateInscriptionParticipantsDtoValidator()
        {
            RuleFor(x => x.ParticipantIds)
                .NotNull().WithMessage("Debes indicar los participantes.");

            RuleForEach(x => x.ParticipantIds)
                .GreaterThan(0).WithMessage("Participante inválido.");
        }
    }
}

namespace DanzaBogota.Application.Validator.User
{
    public class ChangeRoleDtoValidator : AbstractValidator<ChangeRoleDto>
    {
        public ChangeRoleDtoValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("Usuario inválido.");

            RuleFor(x => x.NewRole)
                .NotEmpty().WithMessage("El rol es obligatorio.")
                .Must(r => new[] { "Admin", "Gestor", "Grupo" }.Contains(r))
                .WithMessage("Rol inválido.");
        }
    }
}
