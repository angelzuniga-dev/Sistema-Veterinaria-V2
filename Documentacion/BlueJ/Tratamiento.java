import java.util.ArrayList;
import java.util.List;

/**
 * Tratamiento
 * Tratamiento prescrito dentro de una Consulta, compuesto por uno o mas
 * DetalleTratamiento (medicamentos + dosis).
 */
public class Tratamiento
{
    private static int contador = 1;

    private int idTratamiento;
    private String descripcion;
    private String fechaInicio;
    private String fechaFin;
    private Consulta consulta;
    private List<DetalleTratamiento> detalles;

    public Tratamiento(String descripcion, String fechaInicio, String fechaFin, Consulta consulta)
    {
        this.idTratamiento = contador++;
        this.descripcion = descripcion;
        this.fechaInicio = fechaInicio;
        this.fechaFin = fechaFin;
        this.consulta = consulta;
        this.detalles = new ArrayList<DetalleTratamiento>();
        consulta.agregarTratamiento(this);
    }

    /** Caso de uso: Prescribir Tratamiento (agrega un medicamento con dosis). */
    public void agregarDetalle(Medicamento medicamento, String dosis, int cantidad)
    {
        DetalleTratamiento detalle = new DetalleTratamiento(medicamento, dosis, cantidad);
        detalles.add(detalle);
        medicamento.actualizarStock(-cantidad);
        System.out.println("  -> Prescrito: " + medicamento.getNombre() + " (dosis: " + dosis
                + ", cantidad: " + cantidad + ")");
    }

    public int getIdTratamiento()
    {
        return idTratamiento;
    }

    public String getDescripcion()
    {
        return descripcion;
    }

    public List<DetalleTratamiento> getDetalles()
    {
        return detalles;
    }

    @Override
    public String toString()
    {
        return "Tratamiento #" + idTratamiento + " - " + descripcion
                + " (" + fechaInicio + " a " + fechaFin + ")";
    }
}
