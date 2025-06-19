using System;
using System.Collections.Generic;

namespace KindergartenAPI.Models;

public partial class Consumo
{
    public int Matricula { get; set; }

    public DateOnly Fecha { get; set; }

    public int MenuId { get; set; }

    public virtual Nino MatriculaNavigation { get; set; } = null!;

    public virtual Menu Menu { get; set; } = null!;
}
