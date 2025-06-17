

namespace KindergartenAPI.Models

{

    public class NinoAutorizado
    {
        public int Matricula { get; set; }
        public Nino Nino { get; set; } = null!;

        public string Cedula { get; set; } = null!;
        public Persona Persona { get; set; } = null!;

        public string Relacion { get; set; } = null!;
    }
}

