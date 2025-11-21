using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FerreteriaAPI.Data;
using FerreteriaAPI.DTOs;

namespace FerreteriaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportesController : ControllerBase
    {
        private readonly FerreteriaContext _context;

        public ReportesController(FerreteriaContext context)
        {
            _context = context;
        }

        // GET: api/Reportes/ventas-por-fecha?fechaInicio=2024-01-01&fechaFin=2024-01-31
        [HttpGet("ventas-por-fecha")]
        public async Task<ActionResult<IEnumerable<VentaResponseDTO>>> GetVentasPorFecha(
            [FromQuery] DateTime fechaInicio, 
            [FromQuery] DateTime fechaFin)
        {
            var ventas = await _context.Ventas
                .Include(v => v.Vendedor)
                .Include(v => v.Cliente)
                .Include(v => v.Detalles!)
                    .ThenInclude(d => d.Producto)
                .Where(v => v.Fecha >= fechaInicio && v.Fecha <= fechaFin && v.Estado == "COMPLETADA")
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
                .OrderByDescending(v => v.Fecha)
                .ToListAsync();

            return ventas;
        }

        // GET: api/Reportes/inventario-bajo-stock
        [HttpGet("inventario-bajo-stock")]
        public async Task<ActionResult<IEnumerable<object>>> GetInventarioBajoStock()
        {
            var productosBajoStock = await _context.Productos
                .Where(p => p.Stock < 10 && p.Activo)
                .Select(p => new
                {
                    p.Id,
                    p.Nombre,
                    p.Stock,
                    p.Precio,
                    p.Categoria
                })
                .OrderBy(p => p.Stock)
                .ToListAsync();

            return productosBajoStock;
        }

        // GET: api/Reportes/ventas-por-vendedor
        [HttpGet("ventas-por-vendedor")]
        public async Task<ActionResult<IEnumerable<object>>> GetVentasPorVendedor()
        {
            var ventasPorVendedor = await _context.Ventas
                .Include(v => v.Vendedor)
                .Where(v => v.Estado == "COMPLETADA")
                .GroupBy(v => new { v.VendedorId, v.Vendedor!.Nombre })
                .Select(g => new
                {
                    VendedorId = g.Key.VendedorId,
                    VendedorNombre = g.Key.Nombre,
                    TotalVentas = g.Count(),
                    TotalImporte = g.Sum(v => v.Total)
                })
                .OrderByDescending(x => x.TotalImporte)
                .ToListAsync();

            return ventasPorVendedor;
        }
    }
}