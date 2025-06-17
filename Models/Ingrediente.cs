using System;
using System.Collections.Generic;

namespace KindergartenAPI.Models;

public partial class Ingrediente
{
    public string Nombre { get; set; } = null!;

    public virtual ICollection<Nino> Matriculas { get; set; } = new List<Nino>();

    public virtual ICollection<Plato> Platos { get; set; } = new List<Plato>();
}
