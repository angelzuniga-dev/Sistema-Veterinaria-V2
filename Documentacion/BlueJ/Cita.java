/**
 * Cita
 * Representa el agendamiento de una mascota con un veterinario en una fecha y hora.
 */
public class Cita
{
    private static int contador = 1;

    public static final String ESTADO_PENDIENTE = "PENDIENTE";
    public static final String ESTADO_CANCELADA = "CANCELADA";
    public static final String ESTADO_ATENDIDA = "ATENDIDA";

    private int idCita;
    private String fecha;
    private String hora;
    private String estado;
    private Mascota mascota;
    private Veterinario veterinario;

    public Cita(String fecha, String hora, Mascota mascota, Veterinario veterinario)
    {
        this.idCita = contador++;
        this.fecha = fecha;
        this.hora = hora;
        this.estado = ESTADO_PENDIENTE;
        this.mascota = mascota;
        this.veterinario = veterinario;
        mascota.agregarCita(this);
    }

    /** Caso de uso: Cancelar/Reprogramar Cita. */
    public void cancelarCita()
    {
        this.estado = ESTADO_CANCELADA;
        System.out.println("  -> Cita #" + idCita + " cancelada.");
    }

    public void reprogramarCita(String nuevaFecha, String nuevaHora)
    {
        this.fecha = nuevaFecha;
        this.hora = nuevaHora;
        System.out.println("  -> Cita #" + idCita + " reprogramada a " + nuevaFecha + " " + nuevaHora);
    }

    public void marcarAtendida()
    {
        this.estado = ESTADO_ATENDIDA;
    }

    public int getIdCita()
    {
        return idCita;
    }

    public String getFecha()
    {
        return fecha;
    }

    public String getHora()
    {
        return hora;
    }

    public String getEstado()
    {
        return estado;
    }

    public Mascota getMascota()
    {
        return mascota;
    }

    public Veterinario getVeterinario()
    {
        return veterinario;
    }

    @Override
    public String toString()
    {
        return "Cita #" + idCita + " - " + mascota.getNombre() + " con " + veterinario
                + " el " + fecha + " " + hora + " [" + estado + "]";
    }
}
