namespace SistemaVeterinaria.Dominio
{
    /// <summary>
    /// Comprobante generado para una Consulta (incluye costo de consulta y tratamientos).
    /// </summary>
    public class Factura
    {
        private static int contador = 1;
        public const decimal CostoConsulta = 150.0m;

        public int IdFactura { get; }
        public DateTime Fecha { get; }
        public decimal Total { get; private set; }
        public Cliente Cliente { get; }
        public Consulta Consulta { get; }
        public List<DetalleFactura> Detalles { get; } = new List<DetalleFactura>();

        public Factura(DateTime fecha, Cliente cliente, Consulta consulta)
        {
            IdFactura = contador++;
            Fecha = fecha;
            Cliente = cliente;
            Consulta = consulta;
            GenerarFactura();
        }

        /// <summary>Caso de uso: Generar Factura.</summary>
        private void GenerarFactura()
        {
            Detalles.Add(new DetalleFactura("Consulta veterinaria", 1, CostoConsulta));

            foreach (var t in Consulta.Tratamientos)
            {
                foreach (var dt in t.Detalles)
                {
                    Detalles.Add(new DetalleFactura(
                        $"{dt.Medicamento.Nombre} ({dt.Dosis})",
                        dt.Cantidad,
                        dt.Medicamento.PrecioUnitario));
                }
            }

            Total = CalcularTotal();
        }

        public decimal CalcularTotal()
        {
            decimal suma = 0m;
            foreach (var d in Detalles)
            {
                suma += d.Subtotal;
            }
            return suma;
        }

        public void ImprimirFactura()
        {
            Console.WriteLine($"  ----- FACTURA #{IdFactura} -----");
            Console.WriteLine($"  Cliente : {Cliente.Nombre}");
            Console.WriteLine($"  Fecha   : {Fecha:yyyy-MM-dd}");
            foreach (var d in Detalles)
            {
                Console.WriteLine($"    {d}");
            }
            Console.WriteLine($"  TOTAL   : Q{Total:F2}");
            Console.WriteLine("  --------------------------");
        }
    }
}
