using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using KindergartenAPI.DTOs.Ninos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace KindergartenAPI.Tests.IntegrationTesting
{
    public class NinosEndpointsTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public NinosEndpointsTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Get_Ninos_ReturnsOkAndEmptyList_WhenNoNinos()
        {
            var response = await _client.GetAsync("/api/v1/ninos");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var list = await response.Content.ReadFromJsonAsync<List<NinoReadDto>>();
            list.Should().NotBeNull().And.BeEmpty();
        }

        [Fact]
        public async Task Post_Nino_InvalidDto_ReturnsBadRequest()
        {
            var badDto = new NinoCreateDto
            {
                Nombre = string.Empty,
                FechaNacimiento = DateOnly.FromDateTime(DateTime.Today.AddYears(-3)),
                FechaIngreso = DateOnly.FromDateTime(DateTime.Today),
                CedulaPagador = "000X"
            };

            var response = await _client.PostAsJsonAsync("/api/v1/ninos", badDto);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
            problem.Should().NotBeNull();
            problem!.Errors.Should().ContainKey("Nombre");
        }

        [Fact]
        public async Task FullCrudFlow_Post_Get_Put_Delete_Nino()
        {
            // Seed persona
            var persona = new { Cedula = "40288888888", Nombre = "Pagador1", Direccion = "Dir1", Telefono = "8090000001" };
            var personaRes = await _client.PostAsJsonAsync("/api/v1/personas", persona);
            personaRes.StatusCode.Should().Be(HttpStatusCode.Created);

            // CREATE
            var createDto = new NinoCreateDto
            {
                Nombre = "Test Nino",
                FechaNacimiento = DateOnly.FromDateTime(DateTime.Today.AddYears(-8)),
                FechaIngreso = DateOnly.FromDateTime(DateTime.Today.AddYears(-3)),
                CedulaPagador = persona.Cedula
            };
            var postRes = await _client.PostAsJsonAsync("/api/v1/ninos", createDto);
            postRes.StatusCode.Should().Be(HttpStatusCode.Created);
            var created = await postRes.Content.ReadFromJsonAsync<NinoReadDto>();
            created.Should().NotBeNull();

            var id = created!.Matricula;
            created.Nombre.Should().Be(createDto.Nombre);
            created.FechaBaja.Should().BeNull();

            // GET by id
            var getById = await _client.GetAsync($"/api/v1/ninos/{id}");
            getById.StatusCode.Should().Be(HttpStatusCode.OK);
            var single = await getById.Content.ReadFromJsonAsync<NinoReadDto>();
            single.Should().NotBeNull().And.Match<NinoReadDto>(n => n.Matricula == id);

            // UPDATE
            var updateDto = new NinoUpdateDto
            {
                Nombre = "Updated Nino",
                FechaNacimiento = createDto.FechaNacimiento,
                FechaIngreso = createDto.FechaIngreso,
                FechaBaja = DateOnly.FromDateTime(DateTime.Today),
                CedulaPagador = createDto.CedulaPagador
            };
            var putRes = await _client.PutAsJsonAsync($"/api/v1/ninos/{id}", updateDto);
            putRes.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var getAfterPut = await _client.GetAsync($"/api/v1/ninos/{id}");
            getAfterPut.StatusCode.Should().Be(HttpStatusCode.OK);
            var afterPut = await getAfterPut.Content.ReadFromJsonAsync<NinoReadDto>();
            afterPut!.Nombre.Should().Be(updateDto.Nombre);
            afterPut.FechaBaja.Should().Be(updateDto.FechaBaja);

            // DELETE
            var deleteRes = await _client.DeleteAsync($"/api/v1/ninos/{id}");
            deleteRes.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var getAfterDelete = await _client.GetAsync($"/api/v1/ninos/{id}");
            getAfterDelete.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
