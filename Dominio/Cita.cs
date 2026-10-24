namespace SistemaVeterinaria.Dominio
{
    public enum EstadoCita
    {
        Pendiente,
        Cancelada,
        Atendida
    }

    /// <summary>
    /// Representa el agendamiento de una mascota con un veterinario en fecha y hora.
    /// </summary>
    public class Cita
    {
        private static int contador = 1;

        public int IdCita { get; }
        public DateTime Fecha { get; private set; }
        public string Hora { get; private set; }
        public EstadoCita Estado { get; private set; }
        public Mascota Mascota { get; }
        public Veterinario Veterinario { get; }

        public Cita(DateTime fecha, string hora, Mascota mascota, Veterinario veterinario)
        {
            IdCita = contador++;
            Fecha = fecha;
            Hora = hora;
            Estado = EstadoCita.Pendiente;
            Mascota = mascota;
            Veterinario = veterinario;
            mascota.AgregarCita(this);
        }

        /// <summary>Caso de uso: Cancelar Cita.</summary>
        public void CancelarCita()
        {
            Estado = EstadoCita.Cancelada;
            Console.WriteLine($"  -> Cita #{IdCita} cancelada.");
        }

        /// <summary>Caso de uso: Reprogramar Cita.</summary>
        public void ReprogramarCita(DateTime nuevaFecha, string nuevaHora)
        {
            Fecha = nuevaFecha;
            Hora = nuevaHora;
            Console.WriteLine($"  -> Cita #{IdCita} reprogramada a {nuevaFecha:yyyy-MM-dd} {nuevaHora}");
        }

        public void MarcarAtendida()
        {
            Estado = EstadoCita.Atendida;
        }

        public override string ToString()
        {
            return $"Cita #{IdCita} - {Mascota.Nombre} con {Veterinario} el {Fecha:yyyy-MM-dd} {Hora} [{Estado}]";
        }
    }
}
