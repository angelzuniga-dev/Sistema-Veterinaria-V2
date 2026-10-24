import java.util.ArrayList;
import java.util.List;

/**
 * Cliente
 * Duenio(a) de una o mas mascotas.
 */
public class Cliente
{
    private static int contador = 1;

    private int idCliente;
    private String nombre;
    private String telefono;
    private String direccion;
    private String email;
    private List<Mascota> mascotas;

    public Cliente(String nombre, String telefono, String direccion, String email)
    {
        this.idCliente = contador++;
        this.nombre = nombre;
        this.telefono = telefono;
        this.direccion = direccion;
        this.email = email;
        this.mascotas = new ArrayList<Mascota>();
    }

    /**
     * Caso de uso: Registrar Cliente (creacion via constructor)
     * y Registrar Mascota (asociacion).
     */
    public void agregarMascota(Mascota mascota)
    {
        mascotas.add(mascota);
        System.out.println("  -> Mascota '" + mascota.getNombre() + "' registrada para el cliente " + nombre);
    }

    public void actualizarDatos(String telefono, String direccion, String email)
    {
        this.telefono = telefono;
        this.direccion = direccion;
        this.email = email;
    }

    public int getIdCliente()
    {
        return idCliente;
    }

    public String getNombre()
    {
        return nombre;
    }

    public String getTelefono()
    {
        return telefono;
    }

    public String getEmail()
    {
        return email;
    }

    public List<Mascota> getMascotas()
    {
        return mascotas;
    }

    @Override
    public String toString()
    {
        return "Cliente #" + idCliente + " - " + nombre + " (Tel: " + telefono + ")";
    }
}
