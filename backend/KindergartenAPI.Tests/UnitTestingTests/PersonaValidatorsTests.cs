using FluentAssertions;
using FluentValidation.Results;
using KindergartenAPI.DTOs.Personas;
using KindergartenAPI.Validators;
using Xunit;

namespace KindergartenAPI.Tests.UnitTesting
{
    public class PersonaCreateDtoValidatorTests
    {
        private readonly PersonaCreateDtoValidator _validator = new PersonaCreateDtoValidator();

        [Fact]
        public void Should_Have_Error_For_Invalid_CreateDto()
        {
            var dto = new PersonaCreateDto
            {
                Cedula    = string.Empty,
                Nombre    = string.Empty,
                Direccion = string.Empty,
                Telefono  = "00123454321"
            };

            ValidationResult result = _validator.Validate(dto);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Cedula");
            result.Errors.Should().Contain(e => e.PropertyName == "Nombre");
            result.Errors.Should().Contain(e => e.PropertyName == "Direccion");
            result.Errors.Should().Contain(e => e.PropertyName == "Telefono");
        }

        [Fact]
        public void Should_Pass_When_CreateDto_Is_Valid()
        {
            var dto = new PersonaCreateDto
            {
                Cedula    = "00123456789",
                Nombre    = "John Doe",
                Direccion = "Somewhere",
                Telefono  = "8091234567"
            };

            var result = _validator.Validate(dto);
            result.IsValid.Should().BeTrue();
        }
    }

    public class PersonaUpdateDtoValidatorTests
    {
        private readonly PersonaUpdateDtoValidator _validator = new PersonaUpdateDtoValidator();

        [Fact]
        public void Should_Have_Error_For_Invalid_UpdateDto()
        {
            var dto = new PersonaUpdateDto
            {
                Nombre    = string.Empty,
                Direccion = string.Empty,
                Telefono  = "123"
            };

            ValidationResult result = _validator.Validate(dto);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Nombre");
            result.Errors.Should().Contain(e => e.PropertyName == "Direccion");
            result.Errors.Should().Contain(e => e.PropertyName == "Telefono");
        }

        [Fact]
        public void Should_Pass_When_UpdateDto_Is_Valid()
        {
            var dto = new PersonaUpdateDto
            {
                Nombre    = "Jane Doe",
                Direccion = "Anywhere",
                Telefono  = "8097654321"
            };

            var result = _validator.Validate(dto);
            result.IsValid.Should().BeTrue();
        }
    }
}
