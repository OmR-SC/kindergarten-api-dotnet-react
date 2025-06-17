using System;
using System.Collections.Generic;

namespace KindergartenAPI.Models;

public partial class Nino
{
    public int Matricula { get; set; }

    public string Nombre { get; set; } = null!;

    public DateOnly FechaNacimiento { get; set; }

    public DateOnly FechaIngreso { get; set; }

    public DateOnly? FechaBaja { get; set; }

    public string CedulaPagador { get; set; } = null!;

    public virtual ICollection<Asistencium> Asistencia { get; set; } = new List<Asistencium>();

    public virtual Persona CedulaPagadorNavigation { get; set; } = null!;

    public virtual ICollection<Consumo> Consumos { get; set; } = new List<Consumo>();

    public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();

    public virtual ICollection<Nino_Autorizado> Nino_Autorizados { get; set; } = new List<Nino_Autorizado>();

    public virtual ICollection<Ingrediente> Ingredientes { get; set; } = new List<Ingrediente>();
}
