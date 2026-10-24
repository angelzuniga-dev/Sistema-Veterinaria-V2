import java.util.ArrayList;
import java.util.List;

/**
 * Consulta
 * Atencion medica derivada de una Cita.
 */
public class Consulta
{
    private static int contador = 1;

    private int idConsulta;
    private String fecha;
    private String motivo;
    private String diagnostico;
    private Cita cita;
    private Veterinario veterinario;
    private List<Tratamiento> tratamientos;

    public Consulta(String fecha, String motivo, Cita cita)
    {
        this.idConsulta = contador++;
        this.fecha = fecha;
        this.motivo = motivo;
        this.diagnostico = "";
        this.cita = cita;
        this.veterinario = cita.getVeterinario();
        this.tratamientos = new ArrayList<Tratamiento>();
        cita.marcarAtendida();
        cita.getMascota().getHistoriaClinica().agregarEntrada(this);
    }

    /** Caso de uso: Registrar Consulta (diagnostico). */
    public void registrarDiagnostico(String diagnostico)
    {
        this.diagnostico = diagnostico;
        System.out.println("  -> Diagnostico registrado: " + diagnostico);
    }

    public void agregarTratamiento(Tratamiento tratamiento)
    {
        tratamientos.add(tratamiento);
    }

    public int getIdConsulta()
    {
        return idConsulta;
    }

    public String getMotivo()
    {
        return motivo;
    }

    public String getDiagnostico()
    {
        return diagnostico;
    }

    public Cita getCita()
    {
        return cita;
    }

    public Veterinario getVeterinario()
    {
        return veterinario;
    }

    public List<Tratamiento> getTratamientos()
    {
        return tratamientos;
    }

    @Override
    public String toString()
    {
        return "Consulta #" + idConsulta + " - " + fecha + " - Motivo: " + motivo
                + " - Diagnostico: " + (diagnostico.isEmpty() ? "(pendiente)" : diagnostico);
    }
}
