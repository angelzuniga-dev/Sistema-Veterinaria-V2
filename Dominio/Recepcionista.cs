namespace SistemaVeterinaria.Dominio
{
    /// <summary>
    /// Hereda de Usuario. Gestiona clientes, mascotas, citas y facturacion.
    /// </summary>
    public class Recepcionista : Usuario
    {
        public Recepcionista(string nombre, string usuarioLogin, string password)
            : base(nombre, usuarioLogin, password, RolUsuario.Recepcionista)
        {
        }
    }
}
