namespace SistemaVeterinaria.Dominio
{
    /// <summary>
    /// Clase base para todos los usuarios del sistema.
    /// Caso de uso: Iniciar Sesion.
    /// </summary>
    public class Usuario
    {
        private static int contador = 1;

        public int IdUsuario { get; }
        public string Nombre { get; protected set; }
        public string UsuarioLogin { get; protected set; }
        internal string Password { get; private set; }
        public RolUsuario Rol { get; protected set; }

        public Usuario(string nombre, string usuarioLogin, string password, RolUsuario rol)
        {
            IdUsuario = contador++;
            Nombre = nombre;
            UsuarioLogin = usuarioLogin;
            Password = password;
            Rol = rol;
        }

        /// <summary>
        /// Caso de uso: Iniciar Sesion.
        /// Valida las credenciales recibidas contra las del usuario.
        /// </summary>
        public virtual bool IniciarSesion(string usuarioLogin, string password)
        {
            bool valido = UsuarioLogin == usuarioLogin && Password == password;
            if (valido)
            {
                Console.WriteLine($"  -> Credenciales validas. Bienvenido/a {Nombre} ({Rol})");
            }
            else
            {
                Console.WriteLine($"  -> Credenciales invalidas para el usuario {usuarioLogin}");
            }
            return valido;
        }

        public void CerrarSesion()
        {
            Console.WriteLine($"  -> {Nombre} ha cerrado sesion.");
        }

        public override string ToString()
        {
            return $"Usuario #{IdUsuario} - {Nombre} [{Rol}]";
        }
    }
}
