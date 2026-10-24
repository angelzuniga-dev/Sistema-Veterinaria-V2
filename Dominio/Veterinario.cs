namespace SistemaVeterinaria.Dominio
{
    /// <summary>
    /// Hereda de Usuario. Atiende consultas y prescribe tratamientos.
    /// </summary>
    public class Veterinario : Usuario
    {
        public string Especialidad { get; }
        public string NumeroColegiado { get; }

        public Veterinario(string nombre, string usuarioLogin, string password,
                            string especialidad, string numeroColegiado)
            : base(nombre, usuarioLogin, password, RolUsuario.Veterinario)
        {
            Especialidad = especialidad;
            NumeroColegiado = numeroColegiado;
        }

        public override string ToString()
        {
            return $"Dr(a). {Nombre} - {Especialidad}";
        }
    }
}
