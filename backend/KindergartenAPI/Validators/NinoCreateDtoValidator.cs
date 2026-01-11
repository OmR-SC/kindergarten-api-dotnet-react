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
                .NotNull().WithMessage("La fecha de nacimiento es obligatoria.")
                //.Must(fecha => fecha.ToDateTime(TimeOnly.MinValue) < DateTime.Now)
                .Must(fecha =>
                {
                    if (!fecha.HasValue) return false;
                    return fecha.Value.ToDateTime(TimeOnly.MinValue) < DateTime.Now;
                })
                .WithMessage("La fecha de nacimiento debe ser anterior a la fecha actual.");

            RuleFor(x => x.FechaNacimiento)
                .LessThan(x => x.FechaIngreso)
                .WithMessage("La fecha de nacimiento debe ser anterior a la fecha de ingreso.")
                //.When(x => x.FechaNacimiento.ToDateTime(TimeOnly.MinValue) < DateTime.Now); // only if nacimiento is valid when compare to fecha actual
                .When(x => x.FechaNacimiento.HasValue && x.FechaIngreso.HasValue); // Solo validamos si ambas fechas existen

            RuleFor(n => n.FechaIngreso)
                .NotNull().WithMessage("La fecha de ingreso es obligatoria.")
                .NotEmpty().WithMessage("La fecha de ingreso es obligatoria.")
                //.Must((nino, fechaIngreso) => fechaIngreso >= nino.FechaNacimiento)
                .Must((nino, fechaIngreso) => 
                {
                    if (!fechaIngreso.HasValue || !nino.FechaNacimiento.HasValue) return true;
                    
                    return fechaIngreso.Value >= nino.FechaNacimiento.Value;
                })
                .WithMessage("La fecha de ingreso debe ser igual o posterior a la fecha de nacimiento.");

            RuleFor(n => n.CedulaPagador)
                .NotEmpty().WithMessage("La cédula del pagador es obligatoria.");
        }
    }
}