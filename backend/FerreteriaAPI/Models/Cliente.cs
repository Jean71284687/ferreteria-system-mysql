using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FerreteriaAPI.Models
{
    public class Cliente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(10)]
        public string TipoDocumento { get; set; } = "DNI";

        [Required]
        [StringLength(11)]
        public string NumeroDocumento { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Direccion { get; set; }

        [StringLength(9)]
        public string? Telefono { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}