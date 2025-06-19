using System;
using System.Collections.Generic;

namespace KindergartenAPI.Models;

public partial class Asistencium
{
    public int Id { get; set; }

    public int Matricula { get; set; }

    public DateOnly Fecha { get; set; }

    public virtual Nino MatriculaNavigation { get; set; } = null!;
}
