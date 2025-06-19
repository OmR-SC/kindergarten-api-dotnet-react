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
                .LessThan(DateTime.Now).WithMessage("La fecha de nacimiento debe ser anterior a la fecha actual.");

            RuleFor(n => n.FechaIngreso)
                .NotEmpty().WithMessage("La fecha de ingreso es obligatoria.")
                .GreaterThanOrEqualTo(n => n.FechaNacimiento).WithMessage("La fecha de ingreso debe ser igual o posterior a la fecha de nacimiento.");

            RuleFor(n => n.CedulaPagador)
                .NotEmpty().WithMessage("La cédula del pagador es obligatoria.");
        }
    }
}