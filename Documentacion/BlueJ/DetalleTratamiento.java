/**
 * DetalleTratamiento
 * Linea de un Tratamiento: un medicamento con su dosis y cantidad.
 */
public class DetalleTratamiento
{
    private static int contador = 1;

    private int idDetalle;
    private Medicamento medicamento;
    private String dosis;
    private int cantidad;

    public DetalleTratamiento(Medicamento medicamento, String dosis, int cantidad)
    {
        this.idDetalle = contador++;
        this.medicamento = medicamento;
        this.dosis = dosis;
        this.cantidad = cantidad;
    }

    public int getIdDetalle()
    {
        return idDetalle;
    }

    public Medicamento getMedicamento()
    {
        return medicamento;
    }

    public String getDosis()
    {
        return dosis;
    }

    public int getCantidad()
    {
        return cantidad;
    }

    public double getSubtotal()
    {
        return medicamento.getPrecioUnitario() * cantidad;
    }

    @Override
    public String toString()
    {
        return medicamento.getNombre() + " x" + cantidad + " (" + dosis + ")";
    }
}
