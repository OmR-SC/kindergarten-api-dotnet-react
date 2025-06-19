namespace KindergartenAPI.DTOs.Personas;

public class PersonaCreateDto
{
    public string Cedula { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string Telefono { get; set; } = null!;
    public string Direccion { get; set; } = null!;
}

public class PersonaReadDto
{
    public string Cedula { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string Telefono { get; set; } = null!;
    public string Direccion { get; set; } = null!;
}

    public class PersonaUpdateDto
{
    public string Nombre { get; set; } = null!;
    public string Telefono { get; set; } = null!;
    public string Direccion { get; set; } = null!;

}

