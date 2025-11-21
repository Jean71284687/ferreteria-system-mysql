using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FerreteriaAPI.Models
{
    public class Venta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        [Required]
        public int VendedorId { get; set; }

        [ForeignKey("VendedorId")]
        public Usuario? Vendedor { get; set; }

        public int? ClienteId { get; set; }

        [ForeignKey("ClienteId")]
        public Cliente? Cliente { get; set; }

        [Required]
        [StringLength(20)]
        public string TipoComprobante { get; set; } = "BOLETA";

        [Required]
        [StringLength(20)]
        public string MetodoPago { get; set; } = "EFECTIVO";

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Igv { get; set; } = 0;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Total { get; set; }

        [StringLength(11)]
        public string? RucCliente { get; set; }

        [StringLength(100)]
        public string? RazonSocial { get; set; }

        [StringLength(255)]
        public string? DireccionFiscal { get; set; }

        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = "COMPLETADA";

        public ICollection<DetalleVenta>? Detalles { get; set; }
    }
}