/**
 * Veterinario
 * Hereda de Usuario. Atiende consultas y prescribe tratamientos.
 */
public class Veterinario extends Usuario
{
    private String especialidad;
    private String numeroColegiado;

    public Veterinario(String nombre, String usuarioLogin, String password,
                        String especialidad, String numeroColegiado)
    {
        super(nombre, usuarioLogin, password, RolUsuario.VETERINARIO);
        this.especialidad = especialidad;
        this.numeroColegiado = numeroColegiado;
    }

    public String getEspecialidad()
    {
        return especialidad;
    }

    public String getNumeroColegiado()
    {
        return numeroColegiado;
    }

    @Override
    public String toString()
    {
        return "Dr(a). " + getNombre() + " - " + especialidad;
    }
}
