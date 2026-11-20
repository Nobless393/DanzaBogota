using DanzaBogota.Application.Dto.Group;
using FluentValidation;

namespace DanzaBogota.Application.Validator.Group
{
    public class CreateGroupDtoValidator : AbstractValidator<CreateGroupDto>
    {
        public CreateGroupDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del grupo es obligatorio.")
                .MaximumLength(150).WithMessage("El nombre no puede superar 150 caracteres.");

            RuleFor(x => x.DirectorName)
                .NotEmpty().WithMessage("El nombre del director es obligatorio.")
                .MaximumLength(150).WithMessage("El nombre del director no puede superar 150 caracteres.");

            RuleFor(x => x.Phone)
                .MaximumLength(20).WithMessage("El teléfono no puede superar 20 caracteres.");

            RuleFor(x => x.Email)
                .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
                .WithMessage("Debes ingresar un correo válido.");

            RuleFor(x => x.LocalityId)
                .GreaterThan(0).WithMessage("Debes seleccionar una localidad.");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Debes seleccionar una categoría.");
        }
    }

    public class UpdateGroupDtoValidator : AbstractValidator<UpdateGroupDto>
    {
        public UpdateGroupDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id de grupo inválido.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del grupo es obligatorio.")
                .MaximumLength(150).WithMessage("El nombre no puede superar 150 caracteres.");

            RuleFor(x => x.DirectorName)
                .NotEmpty().WithMessage("El nombre del director es obligatorio.")
                .MaximumLength(150).WithMessage("El nombre del director no puede superar 150 caracteres.");

            RuleFor(x => x.Phone)
                .MaximumLength(20).WithMessage("El teléfono no puede superar 20 caracteres.");

            RuleFor(x => x.Email)
                .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
                .WithMessage("Debes ingresar un correo válido.");

            RuleFor(x => x.LocalityId)
                .GreaterThan(0).WithMessage("Debes seleccionar una localidad.");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Debes seleccionar una categoría.");
        }
    }
}
