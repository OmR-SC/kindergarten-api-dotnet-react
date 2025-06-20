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

            RuleFor(x => x.FechaNacimiento)
                .LessThan(x => x.FechaIngreso)
                .WithMessage("La fecha de nacimiento debe ser anterior a la fecha de ingreso.")
                .When(x => x.FechaNacimiento.ToDateTime(TimeOnly.MinValue) < DateTime.Now); // only if nacimiento is valid when compare to fecha actual

            RuleFor(x => x.FechaIngreso)
                .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
                .WithMessage("La fecha de ingreso no puede ser en el futuro.");

            RuleFor(x => x.FechaBaja)
                .GreaterThan(x => x.FechaIngreso)
                .When(x => x.FechaBaja.HasValue)
                .WithMessage("La fecha de baja debe ser posterior a la fecha de ingreso.");


            RuleFor(x => x.CedulaPagador)
                .NotEmpty()
                .WithMessage("La cédula del pagador es obligatoria.");
        }
    }
}