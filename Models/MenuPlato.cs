namespace KindergartenAPI.Models
{
    public class MenuPlato
    {
        public int MenuId { get; set; }
        public Menu Menu { get; set; } = null!;

        public string PlatoNombre { get; set; } = null!;
        public Plato Plato { get; set; } = null!;
    }
}