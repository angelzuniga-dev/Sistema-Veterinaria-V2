namespace SistemaVeterinaria.Dominio
{
    /// <summary>
    /// Hereda de Usuario. Gestiona usuarios, inventario y reportes.
    /// </summary>
    public class Administrador : Usuario
    {
        public Administrador(string nombre, string usuarioLogin, string password)
            : base(nombre, usuarioLogin, password, RolUsuario.Administrador)
        {
        }
    }
}
