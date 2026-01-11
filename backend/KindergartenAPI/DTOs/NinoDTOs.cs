namespace KindergartenAPI.DTOs.Ninos;

using KindergartenAPI.DTOs.Personas;

public class NinoCreateDto
{
    public string Nombre { get; set; } = null!;
    public DateOnly? FechaNacimiento { get; set; }

    public DateOnly? FechaIngreso { get; set; }

    public string CedulaPagador { get; set; } = null!;
}

public class NinoReadDto
{
    public int Matricula { get; set; }
    public string Nombre { get; set; } = null!;
    public DateOnly FechaNacimiento { get; set; }
    public DateOnly FechaIngreso { get; set; }
    public DateOnly? FechaBaja { get; set; }
    public string CedulaPagador { get; set; } = null!;
}

    public class NinoUpdateDto
    {
        public string Nombre { get; set; } = null!;
        public DateOnly? FechaNacimiento { get; set; }
        public DateOnly? FechaIngreso { get; set; }
        public DateOnly? FechaBaja { get; set; }
        public string CedulaPagador { get; set; } = null!;
    }


public class NinoWithPersonaReadDto
{
    public int Matricula { get; set; }
    public string Nombre { get; set; } = null!;
    public DateOnly FechaNacimiento { get; set; }
    public DateOnly FechaIngreso { get; set; }
    public DateOnly? FechaBaja { get; set; }

    public PersonaReadDto Pagador { get; set; } = null!;
}
