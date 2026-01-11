using System;
using FluentAssertions;
using FluentValidation.Results;
using KindergartenAPI.DTOs.Ninos;
using KindergartenAPI.Validators;
using Xunit;

namespace KindergartenAPI.Tests.UnitTesting
{
    public class NinoCreateDtoValidatorTests
    {
        private readonly NinoCreateDtoValidator _validator = new NinoCreateDtoValidator();

        [Fact]
        public void Should_Have_Error_When_Nombre_Is_Empty()
        {
            var dto = new NinoCreateDto
            {
                Nombre = string.Empty,
                FechaNacimiento = DateOnly.FromDateTime(DateTime.Today.AddYears(-3)),
                FechaIngreso    = DateOnly.FromDateTime(DateTime.Today),
                CedulaPagador   = "12345"
            };

            ValidationResult result = _validator.Validate(dto);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == "Nombre");
        }

        [Fact]
        public void Should_Have_Error_When_FechaNacimiento_Is_In_Future()
        {
            var dto = new NinoCreateDto
            {
                Nombre = "Valid",
                FechaNacimiento = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                FechaIngreso    = DateOnly.FromDateTime(DateTime.Today),
                CedulaPagador   = "12345"
            };

            var result = _validator.Validate(dto);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "FechaNacimiento");
        }

        [Fact]
        public void Should_Have_Error_When_CedulaPagador_Is_Empty()
        {
            var dto = new NinoCreateDto
            {
                Nombre = "Valid",
                FechaNacimiento = DateOnly.FromDateTime(DateTime.Today.AddYears(-3)),
                FechaIngreso    = DateOnly.FromDateTime(DateTime.Today),
                CedulaPagador   = string.Empty
            };

            var result = _validator.Validate(dto);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == "CedulaPagador");
        }

        [Fact]
        public void Should_Pass_When_CreateDto_Is_Valid()
        {
            var dto = new NinoCreateDto
            {
                Nombre = "Valid",
                FechaNacimiento = DateOnly.FromDateTime(DateTime.Today.AddYears(-3)),
                FechaIngreso    = DateOnly.FromDateTime(DateTime.Today),
                CedulaPagador   = "40266666666"
            };

            var result = _validator.Validate(dto);
            result.IsValid.Should().BeTrue();
        }
    }

    public class NinoUpdateDtoValidatorTests
    {
        private readonly NinoUpdateDtoValidator _validator = new NinoUpdateDtoValidator();

        [Fact]
        public void Should_Have_Error_When_FechaBaja_Is_Before_FechaIngreso()
        {
            var dto = new NinoUpdateDto
            {
                Nombre = "Valid",
                FechaNacimiento = DateOnly.FromDateTime(DateTime.Today.AddYears(-4)),
                FechaIngreso    = DateOnly.FromDateTime(DateTime.Today),
                FechaBaja       = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)),
                CedulaPagador   = "40255555555"
            };

            var result = _validator.Validate(dto);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == "FechaBaja");
        }

        [Fact]
        public void Should_Pass_When_UpdateDto_Is_Valid()
        {
            var dto = new NinoUpdateDto
            {
                Nombre = "Valid",
                FechaNacimiento = DateOnly.FromDateTime(DateTime.Today.AddYears(-4)),
                FechaIngreso    = DateOnly.FromDateTime(DateTime.Today),
                FechaBaja       = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                CedulaPagador   = "00155555555"
            };

            var result = _validator.Validate(dto);
            result.IsValid.Should().BeTrue();
        }
    }
}
