/**
 * Recepcionista
 * Hereda de Usuario. Gestiona clientes, mascotas, citas y facturacion.
 */
public class Recepcionista extends Usuario
{
    public Recepcionista(String nombre, String usuarioLogin, String password)
    {
        super(nombre, usuarioLogin, password, RolUsuario.RECEPCIONISTA);
    }
}
