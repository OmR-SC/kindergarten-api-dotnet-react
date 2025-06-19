using System;
using System.Collections.Generic;

namespace KindergartenAPI.Models;

public partial class Factura
{
    public int FacturaId { get; set; }

    public int Matricula { get; set; }

    public int Año { get; set; }

    public byte Mes { get; set; }

    public decimal CosteFijo { get; set; }

    public decimal CosteComidas { get; set; }

    public decimal? Total { get; set; }

    public virtual Nino MatriculaNavigation { get; set; } = null!;
}
