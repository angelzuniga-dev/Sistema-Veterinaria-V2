/**
 * Medicamento
 * Producto del inventario de la clinica.
 */
public class Medicamento
{
    private static int contador = 1;

    private int idMedicamento;
    private String nombre;
    private int stock;
    private double precioUnitario;

    public Medicamento(String nombre, int stock, double precioUnitario)
    {
        this.idMedicamento = contador++;
        this.nombre = nombre;
        this.stock = stock;
        this.precioUnitario = precioUnitario;
    }

    /** Caso de uso: Gestionar Inventario de Medicamentos. */
    public void actualizarStock(int cantidad)
    {
        this.stock += cantidad;
        if (this.stock < 0) {
            this.stock = 0;
            System.out.println("  !! Advertencia: stock insuficiente de " + nombre);
        }
    }

    public int getIdMedicamento()
    {
        return idMedicamento;
    }

    public String getNombre()
    {
        return nombre;
    }

    public int getStock()
    {
        return stock;
    }

    public double getPrecioUnitario()
    {
        return precioUnitario;
    }

    @Override
    public String toString()
    {
        return nombre + " (stock: " + stock + ", precio: Q" + String.format("%.2f", precioUnitario) + ")";
    }
}
