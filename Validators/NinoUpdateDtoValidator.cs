using FluentValidation;
using KindergartenAPI.DTOs.Ninos;

namespace KindergartenAPI.Validators
{
    public class NinoUpdateDtoValidator : AbstractValidator<NinoUpdateDto>
    {
        public NinoUpdateDtoValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres.");

            RuleFor(x => x.FechaNacimiento)
                .LessThan(DateOnly.FromDateTime(DateTime.Today))
                .WithMessage("La fecha de nacimiento debe ser anterior a hoy.");

            RuleFor(x => x.FechaIngreso)
                .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
                .WithMessage("La fecha de ingreso no puede ser en el futuro.");

            RuleFor(x => x.CedulaPagador)
                .NotEmpty()
                .WithMessage("La cédula del pagador es obligatoria.");
        }
    }
}