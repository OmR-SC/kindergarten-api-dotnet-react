using FluentValidation;
using KindergartenAPI.DTOs.Ninos;

namespace KindergartenAPI.Validators
{
    public class NinoCreateDtoValidator : AbstractValidator<NinoCreateDto>
    {
        public NinoCreateDtoValidator()
        {
            RuleFor(n => n.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");

            RuleFor(n => n.FechaNacimiento)
                .NotEmpty().WithMessage("La fecha de nacimiento es obligatoria.")
                .Must(fecha => fecha.ToDateTime(TimeOnly.MinValue) < DateTime.Now)
                .WithMessage("La fecha de nacimiento debe ser anterior a la fecha actual.");

            RuleFor(x => x.FechaNacimiento)
                .LessThan(x => x.FechaIngreso)
                .WithMessage("La fecha de nacimiento debe ser anterior a la fecha de ingreso.")
                .When(x => x.FechaNacimiento.ToDateTime(TimeOnly.MinValue) < DateTime.Now); // only if nacimiento is valid when compare to fecha actual

            RuleFor(n => n.FechaIngreso)
                .NotEmpty().WithMessage("La fecha de ingreso es obligatoria.")
                .Must((nino, fechaIngreso) => fechaIngreso >= nino.FechaNacimiento)
                .WithMessage("La fecha de ingreso debe ser igual o posterior a la fecha de nacimiento.");

            RuleFor(n => n.CedulaPagador)
                .NotEmpty().WithMessage("La cédula del pagador es obligatoria.");
        }
    }
}