namespace FerreteriaAPI.DTOs
{
    public class VentaResponseDTO
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public int VendedorId { get; set; }
        public string? VendedorNombre { get; set; }
        public int? ClienteId { get; set; }
        public string? ClienteNombre { get; set; }
        public string TipoComprobante { get; set; } = string.Empty;
        public string MetodoPago { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal Igv { get; set; }
        public decimal Total { get; set; }
        public string? RucCliente { get; set; }
        public string? RazonSocial { get; set; }
        public string? DireccionFiscal { get; set; }
        public string Estado { get; set; } = string.Empty;
        public List<DetalleVentaResponseDTO> Detalles { get; set; } = new();
    }

    public class DetalleVentaResponseDTO
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string? ProductoNombre { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}