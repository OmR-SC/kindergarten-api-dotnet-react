using System;
using System.Collections.Generic;

namespace KindergartenAPI.Models;

public partial class Plato
{
    public string Nombre { get; set; } = null!;

    public decimal Precio { get; set; }

    public virtual ICollection<Ingrediente> Ingredientes { get; set; } = new List<Ingrediente>();

    public virtual ICollection<Menu> Menus { get; set; } = new List<Menu>();
}
