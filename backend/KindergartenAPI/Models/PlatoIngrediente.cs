namespace KindergartenAPI.Models
{
    public class PlatoIngrediente
    {
        public string PlatoNombre { get; set; } = null!;
        public Plato Plato { get; set; } = null!;

        public string IngredienteNombre { get; set; } = null!;
        public Ingrediente Ingrediente { get; set; } = null!;
    }
}