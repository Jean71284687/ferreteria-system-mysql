using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FerreteriaAPI.Data;
using FerreteriaAPI.Models;
using FerreteriaAPI.DTOs;

namespace FerreteriaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentasController : ControllerBase
    {
        private readonly FerreteriaContext _context;

        public VentasController(FerreteriaContext context)
        {
            _context = context;
        }

        // GET: api/Ventas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VentaResponseDTO>>> GetVentas()
        {
            var ventas = await _context.Ventas
                .Include(v => v.Vendedor)
                .Include(v => v.Cliente)
                .Include(v => v.Detalles!)
                    .ThenInclude(d => d.Producto)
                .Select(v => new VentaResponseDTO
                {
                    Id = v.Id,
                    Fecha = v.Fecha,
                    VendedorId = v.VendedorId,
                    VendedorNombre = v.Vendedor!.Nombre,
                    ClienteId = v.ClienteId,
                    ClienteNombre = v.Cliente!.Nombre,
                    TipoComprobante = v.TipoComprobante,
                    MetodoPago = v.MetodoPago,
                    Subtotal = v.Subtotal,
                    Igv = v.Igv,
                    Total = v.Total,
                    RucCliente = v.RucCliente,
                    RazonSocial = v.RazonSocial,
                    DireccionFiscal = v.DireccionFiscal,
                    Estado = v.Estado,
                    Detalles = v.Detalles!.Select(d => new DetalleVentaResponseDTO
                    {
                        Id = d.Id,
                        ProductoId = d.ProductoId,
                        ProductoNombre = d.Producto!.Nombre,
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario,
                        Subtotal = d.Subtotal
                    }).ToList()
                })
                .ToListAsync();

            return ventas;
        }

        // GET: api/Ventas/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<VentaResponseDTO>> GetVenta(int id)
        {
            var venta = await _context.Ventas
                .Include(v => v.Vendedor)
                .Include(v => v.Cliente)
                .Include(v => v.Detalles!)
                    .ThenInclude(d => d.Producto)
                .Where(v => v.Id == id)
                .Select(v => new VentaResponseDTO
                {
                    Id = v.Id,
                    Fecha = v.Fecha,
                    VendedorId = v.VendedorId,
                    VendedorNombre = v.Vendedor!.Nombre,
                    ClienteId = v.ClienteId,
                    ClienteNombre = v.Cliente!.Nombre,
                    TipoComprobante = v.TipoComprobante,
                    MetodoPago = v.MetodoPago,
                    Subtotal = v.Subtotal,
                    Igv = v.Igv,
                    Total = v.Total,
                    RucCliente = v.RucCliente,
                    RazonSocial = v.RazonSocial,
                    DireccionFiscal = v.DireccionFiscal,
                    Estado = v.Estado,
                    Detalles = v.Detalles!.Select(d => new DetalleVentaResponseDTO
                    {
                        Id = d.Id,
                        ProductoId = d.ProductoId,
                        ProductoNombre = d.Producto!.Nombre,
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario,
                        Subtotal = d.Subtotal
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (venta == null)
            {
                return NotFound();
            }

            return venta;
        }

        // POST: api/Ventas
        [HttpPost]
        public async Task<ActionResult<VentaResponseDTO>> PostVenta(VentaDTO ventaDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Calcular totales
                decimal subtotal = ventaDto.Detalles.Sum(d => d.PrecioUnitario * d.Cantidad);
                decimal igv = subtotal * 0.18m;
                decimal total = subtotal + igv;

                var venta = new Venta
                {
                    Fecha = DateTime.Now,
                    VendedorId = ventaDto.VendedorId,
                    ClienteId = ventaDto.ClienteId,
                    TipoComprobante = ventaDto.TipoComprobante,
                    MetodoPago = ventaDto.MetodoPago,
                    Subtotal = subtotal,
                    Igv = igv,
                    Total = total,
                    RucCliente = ventaDto.RucCliente,
                    RazonSocial = ventaDto.RazonSocial,
                    DireccionFiscal = ventaDto.DireccionFiscal,
                    Estado = "COMPLETADA"
                };

                _context.Ventas.Add(venta);
                await _context.SaveChangesAsync();

                // Crear detalles de venta
                foreach (var detalleDto in ventaDto.Detalles)
                {
                    var producto = await _context.Productos.FindAsync(detalleDto.ProductoId);
                    if (producto == null)
                    {
                        throw new Exception($"Producto con ID {detalleDto.ProductoId} no encontrado");
                    }

                    // Actualizar stock
                    producto.Stock -= detalleDto.Cantidad;
                    if (producto.Stock < 0)
                    {
                        throw new Exception($"Stock insuficiente para el producto: {producto.Nombre}");
                    }

                    var detalle = new DetalleVenta
                    {
                        VentaId = venta.Id,
                        ProductoId = detalleDto.ProductoId,
                        Cantidad = detalleDto.Cantidad,
                        PrecioUnitario = detalleDto.PrecioUnitario,
                        Subtotal = detalleDto.PrecioUnitario * detalleDto.Cantidad
                    };

                    _context.DetalleVentas.Add(detalle);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Devolver respuesta con DTO
                var ventaResponse = await GetVenta(venta.Id);
                return CreatedAtAction(nameof(GetVenta), new { id = venta.Id }, ventaResponse.Value);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest($"Error al procesar la venta: {ex.Message}");
            }
        }

        // PUT: api/Ventas/5/anular
        [HttpPut("{id:int}/anular")]
        public async Task<IActionResult> AnularVenta(int id)
        {
            var venta = await _context.Ventas
                .Include(v => v.Detalles)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venta == null)
            {
                return NotFound();
            }

            if (venta.Estado == "ANULADA")
            {
                return BadRequest("La venta ya está anulada");
            }

            // Revertir stock
            foreach (var detalle in venta.Detalles!)
            {
                var producto = await _context.Productos.FindAsync(detalle.ProductoId);
                if (producto != null)
                {
                    producto.Stock += detalle.Cantidad;
                }
            }

            venta.Estado = "ANULADA";
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VentaExists(int id)
        {
            return _context.Ventas.Any(e => e.Id == id);
        }
    }

    public class VentaDTO
    {
        public int VendedorId { get; set; }
        public int? ClienteId { get; set; }
        public string TipoComprobante { get; set; } = "BOLETA";
        public string MetodoPago { get; set; } = "EFECTIVO";
        public string? RucCliente { get; set; }
        public string? RazonSocial { get; set; }
        public string? DireccionFiscal { get; set; }
        public List<DetalleVentaDTO> Detalles { get; set; } = new();
    }

    public class DetalleVentaDTO
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}