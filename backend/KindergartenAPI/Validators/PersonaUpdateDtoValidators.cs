using FluentValidation;
using KindergartenAPI.DTOs.Personas;

namespace KindergartenAPI.Validators
{
public class PersonaUpdateDtoValidator : AbstractValidator<PersonaUpdateDto>
    {
        public PersonaUpdateDtoValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres.");

            RuleFor(x => x.Direccion)
                .NotEmpty().WithMessage("La dirección es obligatoria.")
                .MaximumLength(200).WithMessage("La dirección no puede exceder 200 caracteres.");

            RuleFor(x => x.Telefono)
                .NotEmpty().WithMessage("El teléfono es obligatorio.")
                .Matches("^[0-9]{7,10}$").WithMessage("El teléfono debe tener entre 7 y 10 dígitos.");
        }
    }
}