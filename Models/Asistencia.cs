
namespace KindergartenAPI.Models

{
    public class Asistencia
    {
        public int Id { get; set; }
        public int Matricula { get; set; }
        public Nino Nino { get; set; } = null!;

        public DateTime Fecha { get; set; }
    }
}