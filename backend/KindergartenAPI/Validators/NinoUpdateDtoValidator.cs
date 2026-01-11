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
                .NotNull().WithMessage("La fecha de nacimiento es obligatoria.")
                //.LessThan(DateOnly.FromDateTime(DateTime.Today))
                .Must(fecha =>
                {
                    if (!fecha.HasValue) return false;
                    return fecha.Value.ToDateTime(TimeOnly.MinValue) < DateTime.Now;
                })
                .WithMessage("La fecha de nacimiento debe ser anterior a hoy.");

            RuleFor(x => x.FechaNacimiento)
                .LessThan(x => x.FechaIngreso)
                .WithMessage("La fecha de nacimiento debe ser anterior a la fecha de ingreso.")
                //.When(x => x.FechaNacimiento.ToDateTime(TimeOnly.MinValue) < DateTime.Now); // only if nacimiento is valid when compare to fecha actual
                .When(x => x.FechaNacimiento.HasValue && x.FechaIngreso.HasValue); // Solo validamos si ambas fechas existen


            RuleFor(x => x.FechaIngreso)
                .NotNull().WithMessage("La fecha de ingreso es obligatoria.")
                .NotEmpty().WithMessage("La fecha de ingreso es obligatoria.")
                //.LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
                //.WithMessage("La fecha de ingreso no puede ser en el futuro.");
                .Must((nino, fechaIngreso) => 
                {
                    if (!fechaIngreso.HasValue || !nino.FechaNacimiento.HasValue) return true;
                    
                    return fechaIngreso.Value >= nino.FechaNacimiento.Value;
                })
                .WithMessage("La fecha de ingreso debe ser igual o posterior a la fecha de nacimiento.");


            RuleFor(x => x.FechaBaja)
                //.GreaterThan(x => x.FechaIngreso)
                //.When(x => x.FechaBaja.HasValue)
                .Must((nino, fechaBaja) =>
                {
                    if (!fechaBaja.HasValue || !nino.FechaIngreso.HasValue) return true;

                    return fechaBaja.Value >= nino.FechaIngreso.Value;
                })
                .When(x => x.FechaBaja.HasValue)
                .WithMessage("La fecha de baja debe ser posterior a la fecha de ingreso.");


            RuleFor(x => x.CedulaPagador)
                .NotEmpty()
                .WithMessage("La cédula del pagador es obligatoria.");
        }
    }
}