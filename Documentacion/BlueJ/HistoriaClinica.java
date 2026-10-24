import java.util.ArrayList;
import java.util.List;

/**
 * HistoriaClinica
 * Registro acumulado de todas las consultas de una mascota.
 */
public class HistoriaClinica
{
    private static int contador = 1;

    private int idHistoria;
    private Mascota mascota;
    private String antecedentes;
    private String observaciones;
    private List<Consulta> consultas;

    public HistoriaClinica(Mascota mascota)
    {
        this.idHistoria = contador++;
        this.mascota = mascota;
        this.antecedentes = "";
        this.observaciones = "";
        this.consultas = new ArrayList<Consulta>();
    }

    /** Caso de uso: Registrar Historia Clinica. */
    public void agregarEntrada(Consulta consulta)
    {
        consultas.add(consulta);
    }

    public void actualizarObservaciones(String observaciones)
    {
        this.observaciones = observaciones;
    }

    /** Caso de uso: Consultar Historial Medico. */
    public List<Consulta> consultarHistorial()
    {
        return consultas;
    }

    public void imprimirHistorial()
    {
        System.out.println("  Historia clinica de " + mascota.getNombre() + " (" + consultas.size() + " consulta(s)):");
        for (Consulta c : consultas) {
            System.out.println("    * " + c);
        }
    }

    public int getIdHistoria()
    {
        return idHistoria;
    }
}
