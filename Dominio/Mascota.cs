namespace SistemaVeterinaria.Dominio
{
    /// <summary>
    /// Paciente de la clinica, asociada a un Cliente y a una HistoriaClinica.
    /// </summary>
    public class Mascota
    {
        private static int contador = 1;

        public int IdMascota { get; }
        public string Nombre { get; }
        public string Especie { get; }
        public string Raza { get; }
        public int Edad { get; }
        public double Peso { get; }
        public Cliente Propietario { get; }
        public HistoriaClinica HistoriaClinica { get; }
        public List<Cita> Citas { get; } = new List<Cita>();

        public Mascota(string nombre, string especie, string raza, int edad, double peso, Cliente propietario)
        {
            IdMascota = contador++;
            Nombre = nombre;
            Especie = especie;
            Raza = raza;
            Edad = edad;
            Peso = peso;
            Propietario = propietario;
            HistoriaClinica = new HistoriaClinica(this);
            propietario.AgregarMascota(this);
        }

        public void AgregarCita(Cita cita)
        {
            Citas.Add(cita);
        }

        public override string ToString()
        {
            return $"Mascota #{IdMascota} - {Nombre} ({Especie}, {Raza}, {Edad} anios)";
        }
    }
}
