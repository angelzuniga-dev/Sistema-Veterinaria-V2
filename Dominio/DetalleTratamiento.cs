namespace SistemaVeterinaria.Dominio
{
    /// <summary>
    /// Linea de un Tratamiento: un medicamento con su dosis y cantidad.
    /// </summary>
    public class DetalleTratamiento
    {
        private static int contador = 1;

        public int IdDetalle { get; }
        public Medicamento Medicamento { get; }
        public string Dosis { get; }
        public int Cantidad { get; }

        public DetalleTratamiento(Medicamento medicamento, string dosis, int cantidad)
        {
            IdDetalle = contador++;
            Medicamento = medicamento;
            Dosis = dosis;
            Cantidad = cantidad;
        }

        public decimal Subtotal => Medicamento.PrecioUnitario * Cantidad;

        public override string ToString()
        {
            return $"{Medicamento.Nombre} x{Cantidad} ({Dosis})";
        }
    }
}
