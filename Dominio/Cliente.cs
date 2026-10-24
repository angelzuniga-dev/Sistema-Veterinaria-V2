namespace SistemaVeterinaria.Dominio
{
    /// <summary>
    /// Duenio(a) de una o mas mascotas.
    /// </summary>
    public class Cliente
    {
        private static int contador = 1;

        public int IdCliente { get; }
        public string Nombre { get; private set; }
        public string Telefono { get; private set; }
        public string Direccion { get; private set; }
        public string Email { get; private set; }
        public List<Mascota> Mascotas { get; } = new List<Mascota>();

        public Cliente(string nombre, string telefono, string direccion, string email)
        {
            IdCliente = contador++;
            Nombre = nombre;
            Telefono = telefono;
            Direccion = direccion;
            Email = email;
        }

        /// <summary>
        /// Caso de uso: Registrar Mascota (asociacion con el cliente).
        /// </summary>
        public void AgregarMascota(Mascota mascota)
        {
            Mascotas.Add(mascota);
            Console.WriteLine($"  -> Mascota '{mascota.Nombre}' registrada para el cliente {Nombre}");
        }

        public void ActualizarDatos(string telefono, string direccion, string email)
        {
            Telefono = telefono;
            Direccion = direccion;
            Email = email;
        }

        public override string ToString()
        {
            return $"Cliente #{IdCliente} - {Nombre} (Tel: {Telefono})";
        }
    }
}
