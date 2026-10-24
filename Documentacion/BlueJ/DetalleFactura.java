/**
 * DetalleFactura
 * Linea de una Factura (concepto, cantidad, precio unitario, subtotal).
 */
public class DetalleFactura
{
    private static int contador = 1;

    private int idDetalle;
    private String concepto;
    private int cantidad;
    private double precioUnitario;
    private double subtotal;

    public DetalleFactura(String concepto, int cantidad, double precioUnitario)
    {
        this.idDetalle = contador++;
        this.concepto = concepto;
        this.cantidad = cantidad;
        this.precioUnitario = precioUnitario;
        this.subtotal = cantidad * precioUnitario;
    }

    public double getSubtotal()
    {
        return subtotal;
    }

    @Override
    public String toString()
    {
        return concepto + " x" + cantidad + " = Q" + String.format("%.2f", subtotal);
    }
}
