using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FerreteriaAPI.Data;

namespace FerreteriaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly FerreteriaContext _context;

        public DashboardController(FerreteriaContext context)
        {
            _context = context;
        }

        // GET: api/Dashboard/estadisticas
        [HttpGet("estadisticas")]
        public async Task<ActionResult<object>> GetEstadisticas()
        {
            var hoy = DateTime.Today;
            var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);

            var ventasHoy = await _context.Ventas
                .Where(v => v.Fecha.Date == hoy && v.Estado == "COMPLETADA")
                .ToListAsync();

            var ventasMes = await _context.Ventas
                .Where(v => v.Fecha >= inicioMes && v.Estado == "COMPLETADA")
                .ToListAsync();

            var productosBajoStock = await _context.Productos
                .Where(p => p.Stock < 10 && p.Activo)
                .CountAsync();

            return new
            {
                VentasHoy = new
                {
                    Cantidad = ventasHoy.Count,
                    Total = ventasHoy.Sum(v => v.Total)
                },
                VentasMes = new
                {
                    Cantidad = ventasMes.Count,
                    Total = ventasMes.Sum(v => v.Total)
                },
                ProductosBajoStock = productosBajoStock,
                TotalClientes = await _context.Clientes.CountAsync(),
                TotalProductos = await _context.Productos.Where(p => p.Activo).CountAsync(),
                TotalUsuarios = await _context.Usuarios.Where(u => u.Activo).CountAsync()
            };
        }

        // GET: api/Dashboard/ventas-ultimos-7-dias
        [HttpGet("ventas-ultimos-7-dias")]
        public async Task<ActionResult<List<object>>> GetVentasUltimos7Dias()
        {
            var fechaInicio = DateTime.Today.AddDays(-6);
            
            var ventasPorDia = await _context.Ventas
                .Where(v => v.Fecha >= fechaInicio && v.Estado == "COMPLETADA")
                .GroupBy(v => v.Fecha.Date)
                .Select(g => new
                {
                    Fecha = g.Key,
                    TotalVentas = g.Sum(v => v.Total),
                    CantidadVentas = g.Count()
                })
                .OrderBy(x => x.Fecha)
                .ToListAsync();

            // Completar días sin ventas
            var resultado = new List<object>();
            for (int i = 0; i < 7; i++)
            {
                var fecha = fechaInicio.AddDays(i);
                var ventaDia = ventasPorDia.FirstOrDefault(v => v.Fecha == fecha);

                resultado.Add(new
                {
                    Fecha = fecha.ToString("dd/MM"),
                    TotalVentas = ventaDia?.TotalVentas ?? 0,
                    CantidadVentas = ventaDia?.CantidadVentas ?? 0
                });
            }

            return resultado;
        }

        // GET: api/Dashboard/productos-mas-vendidos
        [HttpGet("productos-mas-vendidos")]
        public async Task<ActionResult<List<object>>> GetProductosMasVendidos()
        {
            var productosMasVendidos = await _context.DetalleVentas
                .Include(d => d.Producto)
                .Include(d => d.Venta)
                .Where(d => d.Venta!.Estado == "COMPLETADA")
                .GroupBy(d => new { d.ProductoId, d.Producto!.Nombre })
                .Select(g => new
                {
                    ProductoId = g.Key.ProductoId,
                    ProductoNombre = g.Key.Nombre,
                    TotalVendido = g.Sum(d => d.Cantidad),
                    TotalIngresos = g.Sum(d => d.Subtotal)
                })
                .OrderByDescending(x => x.TotalVendido)
                .Take(10)
                .ToListAsync();

            return productosMasVendidos.Cast<object>().ToList();
        }
    }
}