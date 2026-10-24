namespace SistemaVeterinaria.Dominio
{
    /// <summary>
    /// Registro acumulado de todas las consultas de una mascota.
    /// </summary>
    public class HistoriaClinica
    {
        private static int contador = 1;

        public int IdHistoria { get; }
        public Mascota Mascota { get; }
        public string Antecedentes { get; private set; } = string.Empty;
        public string Observaciones { get; private set; } = string.Empty;
        private readonly List<Consulta> consultas = new List<Consulta>();

        public HistoriaClinica(Mascota mascota)
        {
            IdHistoria = contador++;
            Mascota = mascota;
        }

        /// <summary>Caso de uso: Registrar Historia Clinica.</summary>
        public void AgregarEntrada(Consulta consulta)
        {
            consultas.Add(consulta);
        }

        public void ActualizarObservaciones(string observaciones)
        {
            Observaciones = observaciones;
        }

        /// <summary>Caso de uso: Consultar Historial Medico.</summary>
        public List<Consulta> ConsultarHistorial()
        {
            return consultas;
        }

        public void ImprimirHistorial()
        {
            Console.WriteLine($"  Historia clinica de {Mascota.Nombre} ({consultas.Count} consulta(s)):");
            foreach (var c in consultas)
            {
                Console.WriteLine($"    * {c}");
            }
        }
    }
}
