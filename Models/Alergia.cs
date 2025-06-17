
namespace KindergartenAPI.Models

{
    public class Alergia
    {
        public int Matricula { get; set; }
        public Nino Nino { get; set; } = null!;

        public string IngredienteNombre { get; set; } = null!;
        public Ingrediente Ingrediente { get; set; } = null!;
    }

}