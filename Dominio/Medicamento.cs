namespace SistemaVeterinaria.Dominio
{
    /// <summary>
    /// Producto del inventario de la clinica.
    /// </summary>
    public class Medicamento
    {
        private static int contador = 1;

        public int IdMedicamento { get; }
        public string Nombre { get; }
        public int Stock { get; private set; }
        public decimal PrecioUnitario { get; }

        public Medicamento(string nombre, int stock, decimal precioUnitario)
        {
            IdMedicamento = contador++;
            Nombre = nombre;
            Stock = stock;
            PrecioUnitario = precioUnitario;
        }

        /// <summary>Caso de uso: Gestionar Inventario de Medicamentos.</summary>
        public void ActualizarStock(int cantidad)
        {
            Stock += cantidad;
            if (Stock < 0)
            {
                Stock = 0;
                Console.WriteLine($"  !! Advertencia: stock insuficiente de {Nombre}");
            }
        }

        public override string ToString()
        {
            return $"{Nombre} (stock: {Stock}, precio: Q{PrecioUnitario:F2})";
        }
    }
}
