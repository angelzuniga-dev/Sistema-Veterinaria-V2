import java.util.ArrayList;
import java.util.List;

/**
 * Factura
 * Comprobante generado para una Consulta (incluye costo de consulta y tratamientos).
 */
public class Factura
{
    private static int contador = 1;
    private static final double COSTO_CONSULTA = 150.0;

    private int idFactura;
    private String fecha;
    private double total;
    private Cliente cliente;
    private Consulta consulta;
    private List<DetalleFactura> detalles;

    public Factura(String fecha, Cliente cliente, Consulta consulta)
    {
        this.idFactura = contador++;
        this.fecha = fecha;
        this.cliente = cliente;
        this.consulta = consulta;
        this.detalles = new ArrayList<DetalleFactura>();
        generarFactura();
    }

    /** Caso de uso: Generar Factura. */
    private void generarFactura()
    {
        detalles.add(new DetalleFactura("Consulta veterinaria", 1, COSTO_CONSULTA));

        for (Tratamiento t : consulta.getTratamientos()) {
            for (DetalleTratamiento dt : t.getDetalles()) {
                detalles.add(new DetalleFactura(
                        dt.getMedicamento().getNombre() + " (" + dt.getDosis() + ")",
                        dt.getCantidad(),
                        dt.getMedicamento().getPrecioUnitario()));
            }
        }
        this.total = calcularTotal();
    }

    public double calcularTotal()
    {
        double suma = 0.0;
        for (DetalleFactura d : detalles) {
            suma += d.getSubtotal();
        }
        return suma;
    }

    public void imprimirFactura()
    {
        System.out.println("  ----- FACTURA #" + idFactura + " -----");
        System.out.println("  Cliente : " + cliente.getNombre());
        System.out.println("  Fecha   : " + fecha);
        for (DetalleFactura d : detalles) {
            System.out.println("    " + d);
        }
        System.out.println("  TOTAL   : Q" + String.format("%.2f", total));
        System.out.println("  --------------------------");
    }

    public int getIdFactura()
    {
        return idFactura;
    }

    public double getTotal()
    {
        return total;
    }

    public List<DetalleFactura> getDetalles()
    {
        return detalles;
    }
}
