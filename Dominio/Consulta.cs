namespace SistemaVeterinaria.Dominio
{
    /// <summary>
    /// Atencion medica derivada de una Cita.
    /// </summary>
    public class Consulta
    {
        private static int contador = 1;

        public int IdConsulta { get; }
        public DateTime Fecha { get; }
        public string Motivo { get; }
        public string Diagnostico { get; private set; } = string.Empty;
        public Cita Cita { get; }
        public Veterinario Veterinario { get; }
        public List<Tratamiento> Tratamientos { get; } = new List<Tratamiento>();

        public Consulta(DateTime fecha, string motivo, Cita cita)
        {
            IdConsulta = contador++;
            Fecha = fecha;
            Motivo = motivo;
            Cita = cita;
            Veterinario = cita.Veterinario;
            cita.MarcarAtendida();
            cita.Mascota.HistoriaClinica.AgregarEntrada(this);
        }

        /// <summary>Caso de uso: Registrar Consulta (diagnostico).</summary>
        public void RegistrarDiagnostico(string diagnostico)
        {
            Diagnostico = diagnostico;
            Console.WriteLine($"  -> Diagnostico registrado: {diagnostico}");
        }

        public void AgregarTratamiento(Tratamiento tratamiento)
        {
            Tratamientos.Add(tratamiento);
        }

        public override string ToString()
        {
            string diag = string.IsNullOrEmpty(Diagnostico) ? "(pendiente)" : Diagnostico;
            return $"Consulta #{IdConsulta} - {Fecha:yyyy-MM-dd} - Motivo: {Motivo} - Diagnostico: {diag}";
        }
    }
}
