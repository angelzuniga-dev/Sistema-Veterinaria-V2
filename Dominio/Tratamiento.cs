namespace SistemaVeterinaria.Dominio
{
    /// <summary>
    /// Tratamiento prescrito dentro de una Consulta, compuesto por uno o mas
    /// DetalleTratamiento (medicamentos + dosis).
    /// </summary>
    public class Tratamiento
    {
        private static int contador = 1;

        public int IdTratamiento { get; }
        public string Descripcion { get; }
        public DateTime FechaInicio { get; }
        public DateTime FechaFin { get; }
        public Consulta Consulta { get; }
        public List<DetalleTratamiento> Detalles { get; } = new List<DetalleTratamiento>();

        public Tratamiento(string descripcion, DateTime fechaInicio, DateTime fechaFin, Consulta consulta)
        {
            IdTratamiento = contador++;
            Descripcion = descripcion;
            FechaInicio = fechaInicio;
            FechaFin = fechaFin;
            Consulta = consulta;
            consulta.AgregarTratamiento(this);
        }

        /// <summary>Caso de uso: Prescribir Tratamiento (agrega un medicamento con dosis).</summary>
        public void AgregarDetalle(Medicamento medicamento, string dosis, int cantidad)
        {
            var detalle = new DetalleTratamiento(medicamento, dosis, cantidad);
            Detalles.Add(detalle);
            medicamento.ActualizarStock(-cantidad);
            Console.WriteLine($"  -> Prescrito: {medicamento.Nombre} (dosis: {dosis}, cantidad: {cantidad})");
        }

        public override string ToString()
        {
            return $"Tratamiento #{IdTratamiento} - {Descripcion} ({FechaInicio:yyyy-MM-dd} a {FechaFin:yyyy-MM-dd})";
        }
    }
}
