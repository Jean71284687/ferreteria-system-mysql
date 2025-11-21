using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FerreteriaAPI.Data;
using FerreteriaAPI.DTOs;

namespace FerreteriaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComprobantesController : ControllerBase
    {
        private readonly FerreteriaContext _context;

        public ComprobantesController(FerreteriaContext context)
        {
            _context = context;
        }

        // GET: api/Comprobantes/venta/2
        [HttpGet("venta/{id:int}")]
        public async Task<ActionResult<VentaResponseDTO>> GetComprobante(int id)
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
                return NotFound("Venta no encontrada");
            }

            return venta;
        }
    }
}