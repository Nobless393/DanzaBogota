using DanzaBogota.Application.Dto.Event;
using FluentValidation;

namespace DanzaBogota.Application.Validator.Event
{
    public class CreateEventDtoValidator : AbstractValidator<CreateEventDto>
    {
        public CreateEventDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("El título es obligatorio.")
                .MaximumLength(200).WithMessage("El título no puede superar 200 caracteres.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La descripción es obligatoria.");

            RuleFor(x => x.LocationName)
                .NotEmpty().WithMessage("El lugar es obligatorio.")
                .MaximumLength(200).WithMessage("El lugar no puede superar 200 caracteres.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("La dirección es obligatoria.")
                .MaximumLength(300).WithMessage("La dirección no puede superar 300 caracteres.");

            RuleFor(x => x.LocalityId)
                .GreaterThan(0).WithMessage("Debes seleccionar una localidad.");

            RuleFor(x => x.MaxParticipants)
                .GreaterThan(0).WithMessage("El máximo de participantes debe ser mayor a 0.");

            RuleFor(x => x.MaxParticipantsPerGroup)
                .GreaterThan(0).WithMessage("El máximo por grupo debe ser mayor a 0.");

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate).WithMessage("La fecha de fin debe ser posterior a la de inicio.");

            RuleFor(x => x.EventTypeIds)
                .NotEmpty().WithMessage("Debes seleccionar al menos un tipo de evento.");

            RuleFor(x => x.CategoryIds)
                .NotEmpty().WithMessage("Debes seleccionar al menos una categoría.");
        }
    }

    public class UpdateEventDtoValidator : AbstractValidator<UpdateEventDto>
    {
        public UpdateEventDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id de evento inválido.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("El título es obligatorio.")
                .MaximumLength(200).WithMessage("El título no puede superar 200 caracteres.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La descripción es obligatoria.");

            RuleFor(x => x.LocationName)
                .NotEmpty().WithMessage("El lugar es obligatorio.")
                .MaximumLength(200).WithMessage("El lugar no puede superar 200 caracteres.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("La dirección es obligatoria.")
                .MaximumLength(300).WithMessage("La dirección no puede superar 300 caracteres.");

            RuleFor(x => x.LocalityId)
                .GreaterThan(0).WithMessage("Debes seleccionar una localidad.");

            RuleFor(x => x.MaxParticipants)
                .GreaterThan(0).WithMessage("El máximo de participantes debe ser mayor a 0.");

            RuleFor(x => x.MaxParticipantsPerGroup)
                .GreaterThan(0).WithMessage("El máximo por grupo debe ser mayor a 0.");

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate).WithMessage("La fecha de fin debe ser posterior a la de inicio.");

            RuleFor(x => x.EventTypeIds)
                .NotEmpty().WithMessage("Debes seleccionar al menos un tipo de evento.");

            RuleFor(x => x.CategoryIds)
                .NotEmpty().WithMessage("Debes seleccionar al menos una categoría.");
        }
    }
}
