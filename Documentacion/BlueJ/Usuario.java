/**
 * Usuario
 * Clase base para todos los usuarios del sistema (caso de uso: Iniciar Sesion).
 */
public class Usuario
{
    private static int contador = 1;

    private int idUsuario;
    private String nombre;
    private String usuarioLogin;
    private String password;
    private RolUsuario rol;

    public Usuario(String nombre, String usuarioLogin, String password, RolUsuario rol)
    {
        this.idUsuario = contador++;
        this.nombre = nombre;
        this.usuarioLogin = usuarioLogin;
        this.password = password;
        this.rol = rol;
    }

    /**
     * Caso de uso: Iniciar Sesion.
     * Valida las credenciales recibidas contra las del usuario.
     */
    public boolean iniciarSesion(String usuarioLogin, String password)
    {
        boolean valido = this.usuarioLogin.equals(usuarioLogin) && this.password.equals(password);
        if (valido) {
            System.out.println("  -> Credenciales validas. Bienvenido/a " + nombre + " (" + rol + ")");
        } else {
            System.out.println("  -> Credenciales invalidas para el usuario " + usuarioLogin);
        }
        return valido;
    }

    public void cerrarSesion()
    {
        System.out.println("  -> " + nombre + " ha cerrado sesion.");
    }

    public int getIdUsuario()
    {
        return idUsuario;
    }

    public String getNombre()
    {
        return nombre;
    }

    public String getUsuarioLogin()
    {
        return usuarioLogin;
    }

    public RolUsuario getRol()
    {
        return rol;
    }

    @Override
    public String toString()
    {
        return "Usuario #" + idUsuario + " - " + nombre + " [" + rol + "]";
    }
}
