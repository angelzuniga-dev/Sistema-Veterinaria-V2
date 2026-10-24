namespace SistemaVeterinaria.Dominio
{
    /// <summary>
    /// Linea de una Factura (concepto, cantidad, precio unitario, subtotal).
    /// </summary>
    public class DetalleFactura
    {
        private static int contador = 1;

        public int IdDetalle { get; }
        public string Concepto { get; }
        public int Cantidad { get; }
        public decimal PrecioUnitario { get; }
        public decimal Subtotal { get; }

        public DetalleFactura(string concepto, int cantidad, decimal precioUnitario)
        {
            IdDetalle = contador++;
            Concepto = concepto;
            Cantidad = cantidad;
            PrecioUnitario = precioUnitario;
            Subtotal = cantidad * precioUnitario;
        }

        public override string ToString()
        {
            return $"{Concepto} x{Cantidad} = Q{Subtotal:F2}";
        }
    }
}
