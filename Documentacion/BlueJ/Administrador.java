/**
 * Administrador
 * Hereda de Usuario. Gestiona usuarios, inventario y reportes.
 */
public class Administrador extends Usuario
{
    public Administrador(String nombre, String usuarioLogin, String password)
    {
        super(nombre, usuarioLogin, password, RolUsuario.ADMINISTRADOR);
    }
}
