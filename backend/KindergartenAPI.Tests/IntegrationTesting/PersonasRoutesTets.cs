using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using KindergartenAPI.DTOs.Personas;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace KindergartenAPI.Tests.IntegrationTesting
{
    public class PersonasEndpointsTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public PersonasEndpointsTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Get_Personas_ReturnsOkAndEmptyList_WhenNoPersonas()
        {
            var response = await _client.GetAsync("/api/v1/personas");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var list = await response.Content.ReadFromJsonAsync<List<PersonaReadDto>>();
            list.Should().NotBeNull().And.BeEmpty();
        }

        [Fact]
        public async Task Post_Persona_InvalidDto_ReturnsBadRequest()
        {
            var badDto = new PersonaCreateDto
            {
                Cedula = string.Empty,
                Nombre = "",
                Direccion = "",
                Telefono = "123" // invalid, less than 7 digits
            };

            var response = await _client.PostAsJsonAsync("/api/v1/personas", badDto);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
            problem.Should().NotBeNull();
            problem!.Errors.Should().ContainKey("Cedula");
            problem.Errors.Should().ContainKey("Nombre");
            problem.Errors.Should().ContainKey("Direccion");
            problem.Errors.Should().ContainKey("Telefono");
        }

        [Fact]
        public async Task FullCrudFlow_Post_Get_Put_Delete_Persona()
        {
            // CREATE
            var createDto = new PersonaCreateDto
            {
                Cedula = "40277777777",
                Nombre = "Juan Perez",
                Direccion = "Calle 123",
                Telefono = "8091234567"
            };
            var postRes = await _client.PostAsJsonAsync("/api/v1/personas", createDto);
            postRes.StatusCode.Should().Be(HttpStatusCode.Created);
            var created = await postRes.Content.ReadFromJsonAsync<PersonaReadDto>();
            created.Should().NotBeNull();
            created!.Cedula.Should().Be(createDto.Cedula);

            // GET all
            var getAll = await _client.GetAsync("/api/v1/personas");
            getAll.StatusCode.Should().Be(HttpStatusCode.OK);
            var list = await getAll.Content.ReadFromJsonAsync<List<PersonaReadDto>>();
            list.Should().ContainSingle(p => p.Cedula == createDto.Cedula);

            // GET by id
            var getById = await _client.GetAsync($"/api/v1/personas/{createDto.Cedula}");
            getById.StatusCode.Should().Be(HttpStatusCode.OK);
            var single = await getById.Content.ReadFromJsonAsync<PersonaReadDto>();
            single.Should().NotBeNull().And.Match<PersonaReadDto>(p => p.Cedula == createDto.Cedula);

            // UPDATE
            var updateDto = new PersonaUpdateDto
            {
                Nombre = "Juan Actualizado",
                Direccion = createDto.Direccion,
                Telefono = createDto.Telefono
            };
            var putRes = await _client.PutAsJsonAsync($"/api/v1/personas/{createDto.Cedula}", updateDto);
            putRes.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var getAfterPut = await _client.GetAsync($"/api/v1/personas/{createDto.Cedula}");
            var afterPut = await getAfterPut.Content.ReadFromJsonAsync<PersonaReadDto>();
            afterPut!.Nombre.Should().Be(updateDto.Nombre);

            // DELETE
            var deleteRes = await _client.DeleteAsync($"/api/v1/personas/{createDto.Cedula}");
            deleteRes.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var getAfterDelete = await _client.GetAsync($"/api/v1/personas/{createDto.Cedula}");
            getAfterDelete.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
