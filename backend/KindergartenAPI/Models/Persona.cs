using System;
using System.Collections.Generic;

namespace KindergartenAPI.Models;

public partial class Persona
{
    public string Cedula { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string Direccion { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public string? CuentaCorriente { get; set; }

    public virtual ICollection<Nino_Autorizado> Nino_Autorizados { get; set; } = new List<Nino_Autorizado>();

    public virtual ICollection<Nino> Ninos { get; set; } = new List<Nino>();
}
