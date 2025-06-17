using System;
using System.Collections.Generic;

namespace KindergartenAPI.Models;

public partial class Menu
{
    public int MenuId { get; set; }

    public string? Descripcion { get; set; }

    public virtual ICollection<Consumo> Consumos { get; set; } = new List<Consumo>();

    public virtual ICollection<Plato> Platos { get; set; } = new List<Plato>();
}
