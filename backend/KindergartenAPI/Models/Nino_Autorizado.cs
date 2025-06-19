using System;
using System.Collections.Generic;

namespace KindergartenAPI.Models;

public partial class Nino_Autorizado
{
    public int Matricula { get; set; }

    public string Cedula { get; set; } = null!;

    public string Relacion { get; set; } = null!;

    public virtual Persona CedulaNavigation { get; set; } = null!;

    public virtual Nino MatriculaNavigation { get; set; } = null!;
}
