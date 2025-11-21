import React, { useState, useEffect } from 'react';
import { 
  Container, 
  Grid, 
  Card, 
  CardContent, 
  Typography, 
  Paper,
  Box,
  Alert,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Chip,
  LinearProgress
} from '@mui/material';
import { 
  ShoppingCart, 
  People, 
  Inventory, 
  TrendingUp,
  Warning,
  AttachMoney,
  BarChart,
  Star,
  Build,
  LocalShipping,
  Security
} from '@mui/icons-material';
import { dashboardAPI, reportesAPI } from '../../services/api.js';

const Dashboard = () => {
  const [estadisticas, setEstadisticas] = useState(null);
  const [ventas7Dias, setVentas7Dias] = useState([]);
  const [productosMasVendidos, setProductosMasVendidos] = useState([]);
  const [inventarioBajo, setInventarioBajo] = useState([]);
  const [ventasPorVendedor, setVentasPorVendedor] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    cargarDatos();
  }, []);

  const cargarDatos = async () => {
    try {
      const [
        estadisticasRes, 
        ventasRes, 
        productosRes, 
        inventarioRes,
        vendedoresRes
      ] = await Promise.all([
        dashboardAPI.getEstadisticas(),
        dashboardAPI.getVentasUltimos7Dias(),
        dashboardAPI.getProductosMasVendidos(),
        reportesAPI.getInventarioBajoStock(),
        reportesAPI.getVentasPorVendedor()
      ]);
      
      setEstadisticas(estadisticasRes.data);
      setVentas7Dias(ventasRes.data);
      setProductosMasVendidos(productosRes.data);
      setInventarioBajo(inventarioRes.data);
      setVentasPorVendedor(vendedoresRes.data);
    } catch (error) {
      console.error('Error cargando datos:', error);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '50vh' }}>
        <Box sx={{ textAlign: 'center' }}>
          <LinearProgress sx={{ width: 200, mb: 2 }} />
          <Typography variant="h6" color="primary">
            Cargando estadísticas...
          </Typography>
        </Box>
      </Box>
    );
  }

  return (
    <Container maxWidth="xl" sx={{ mt: 3, mb: 4, px: { xs: 2, sm: 3 } }}>
      {/* HEADER CENTRADO */}
      <Box sx={{ 
        textAlign: 'center', 
        mb: 6,
        background: 'linear-gradient(135deg, #2c3e50 0%, #3498db 100%)',
        color: 'white',
        py: 4,
        borderRadius: 2,
        boxShadow: 3
      }}>
        <Build sx={{ fontSize: 48, mb: 2 }} />
        <Typography variant="h3" component="h1" gutterBottom fontWeight="bold">
          FERRETERÍA PERÚ
        </Typography>
        <Typography variant="h5" sx={{ opacity: 0.9 }}>
          Sistema de Gestión Integral
        </Typography>
      </Box>

      {/* TARJETAS DE ESTADÍSTICAS - CENTRADAS Y RESPONSIVE */}
      <Grid container spacing={3} sx={{ mb: 6, justifyContent: 'center' }}>
        {/* VENTAS HOY */}
        <Grid item xs={12} sm={6} md={3} sx={{ display: 'flex', justifyContent: 'center' }}>
          <Card sx={{ 
            width: '100%',
            maxWidth: 280,
            background: 'linear-gradient(135deg, #e74c3c 0%, #c0392b 100%)',
            color: 'white',
            textAlign: 'center',
            transition: 'transform 0.3s',
            '&:hover': { transform: 'translateY(-5px)' }
          }}>
            <CardContent sx={{ p: 3 }}>
              <ShoppingCart sx={{ fontSize: 40, mb: 2 }} />
              <Typography variant="h6" gutterBottom fontWeight="medium">
                VENTAS HOY
              </Typography>
              <Typography variant="h3" fontWeight="bold" gutterBottom>
                S/ {estadisticas?.ventasHoy?.total?.toFixed(2) || '0.00'}
              </Typography>
              <Chip 
                label={`${estadisticas?.ventasHoy?.cantidad || 0} transacciones`} 
                size="small" 
                sx={{ background: 'rgba(255,255,255,0.2)', color: 'white' }}
              />
            </CardContent>
          </Card>
        </Grid>

        {/* VENTAS MES */}
        <Grid item xs={12} sm={6} md={3} sx={{ display: 'flex', justifyContent: 'center' }}>
          <Card sx={{ 
            width: '100%',
            maxWidth: 280,
            background: 'linear-gradient(135deg, #27ae60 0%, #229954 100%)',
            color: 'white',
            textAlign: 'center',
            transition: 'transform 0.3s',
            '&:hover': { transform: 'translateY(-5px)' }
          }}>
            <CardContent sx={{ p: 3 }}>
              <TrendingUp sx={{ fontSize: 40, mb: 2 }} />
              <Typography variant="h6" gutterBottom fontWeight="medium">
                VENTAS MES
              </Typography>
              <Typography variant="h3" fontWeight="bold" gutterBottom>
                S/ {estadisticas?.ventasMes?.total?.toFixed(2) || '0.00'}
              </Typography>
              <Chip 
                label={`${estadisticas?.ventasMes?.cantidad || 0} ventas`} 
                size="small" 
                sx={{ background: 'rgba(255,255,255,0.2)', color: 'white' }}
              />
            </CardContent>
          </Card>
        </Grid>

        {/* INVENTARIO */}
        <Grid item xs={12} sm={6} md={3} sx={{ display: 'flex', justifyContent: 'center' }}>
          <Card sx={{ 
            width: '100%',
            maxWidth: 280,
            background: 'linear-gradient(135deg, #2980b9 0%, #1f618d 100%)',
            color: 'white',
            textAlign: 'center',
            transition: 'transform 0.3s',
            '&:hover': { transform: 'translateY(-5px)' }
          }}>
            <CardContent sx={{ p: 3 }}>
              <Inventory sx={{ fontSize: 40, mb: 2 }} />
              <Typography variant="h6" gutterBottom fontWeight="medium">
                INVENTARIO
              </Typography>
              <Typography variant="h3" fontWeight="bold" gutterBottom>
                {estadisticas?.totalProductos || 0}
              </Typography>
              <Chip 
                label={`${estadisticas?.productosBajoStock || 0} bajo stock`} 
                size="small" 
                sx={{ background: 'rgba(255,255,255,0.2)', color: 'white' }}
              />
            </CardContent>
          </Card>
        </Grid>

        {/* CLIENTES */}
        <Grid item xs={12} sm={6} md={3} sx={{ display: 'flex', justifyContent: 'center' }}>
          <Card sx={{ 
            width: '100%',
            maxWidth: 280,
            background: 'linear-gradient(135deg, #8e44ad 0%, #732d91 100%)',
            color: 'white',
            textAlign: 'center',
            transition: 'transform 0.3s',
            '&:hover': { transform: 'translateY(-5px)' }
          }}>
            <CardContent sx={{ p: 3 }}>
              <People sx={{ fontSize: 40, mb: 2 }} />
              <Typography variant="h6" gutterBottom fontWeight="medium">
                CLIENTES
              </Typography>
              <Typography variant="h3" fontWeight="bold" gutterBottom>
                {estadisticas?.totalClientes || 0}
              </Typography>
              <Chip 
                label="registrados" 
                size="small" 
                sx={{ background: 'rgba(255,255,255,0.2)', color: 'white' }}
              />
            </CardContent>
          </Card>
        </Grid>
      </Grid>

      {/* ALERTAS */}
      {estadisticas?.productosBajoStock > 0 && (
        <Alert 
          severity="warning" 
          icon={<Warning />}
          sx={{ 
            mb: 4, 
            borderRadius: 2,
            '& .MuiAlert-message': { textAlign: 'center', width: '100%' }
          }}
        >
          <Typography variant="h6" fontWeight="bold">
            ⚠️ ALERTA: {estadisticas.productosBajoStock} PRODUCTOS CON STOCK BAJO
          </Typography>
        </Alert>
      )}

      {/* TABLAS ESTADÍSTICAS - MEJOR DISEÑO */}
      <Grid container spacing={3} sx={{ justifyContent: 'center' }}>
        
        {/* PRODUCTOS MÁS VENDIDOS */}
        <Grid item xs={12} lg={5}>
          <Paper sx={{ 
            p: 3, 
            height: '400px', 
            overflow: 'auto',
            textAlign: 'center',
            border: '2px solid #e3f2fd',
            borderRadius: 3
          }}>
            <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center', mb: 3 }}>
              <Star sx={{ color: '#ff9800', mr: 1, fontSize: 28 }} />
              <Typography variant="h5" fontWeight="bold" color="#2c3e50">
                PRODUCTOS MÁS VENDIDOS
              </Typography>
            </Box>
            <TableContainer>
              <Table size="small">
                <TableHead>
                  <TableRow sx={{ background: '#f5f5f5' }}>
                    <TableCell><strong>PRODUCTO</strong></TableCell>
                    <TableCell align="center"><strong>CANTIDAD</strong></TableCell>
                    <TableCell align="right"><strong>TOTAL</strong></TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {productosMasVendidos.slice(0, 8).map((producto, index) => (
                    <TableRow key={producto.productoId} hover sx={{ '&:last-child td': { border: 0 } }}>
                      <TableCell>
                        <Typography variant="body2" fontWeight="medium">
                          {producto.productoNombre}
                        </Typography>
                      </TableCell>
                      <TableCell align="center">
                        <Chip 
                          label={producto.totalVendido} 
                          size="small" 
                          color="primary" 
                          variant="outlined"
                        />
                      </TableCell>
                      <TableCell align="right">
                        <Typography variant="body2" fontWeight="bold" color="green">
                          S/ {producto.totalIngresos?.toFixed(2)}
                        </Typography>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </TableContainer>
          </Paper>
        </Grid>

        {/* VENTAS POR VENDEDOR */}
        <Grid item xs={12} lg={5}>
          <Paper sx={{ 
            p: 3, 
            height: '400px', 
            overflow: 'auto',
            textAlign: 'center',
            border: '2px solid #e8f5e8',
            borderRadius: 3
          }}>
            <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center', mb: 3 }}>
              <LocalShipping sx={{ color: '#4caf50', mr: 1, fontSize: 28 }} />
              <Typography variant="h5" fontWeight="bold" color="#2c3e50">
                VENTAS POR VENDEDOR
              </Typography>
            </Box>
            <TableContainer>
              <Table size="small">
                <TableHead>
                  <TableRow sx={{ background: '#f5f5f5' }}>
                    <TableCell><strong>VENDEDOR</strong></TableCell>
                    <TableCell align="center"><strong>VENTAS</strong></TableCell>
                    <TableCell align="right"><strong>TOTAL</strong></TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {ventasPorVendedor.map((vendedor, index) => (
                    <TableRow key={vendedor.vendedorId} hover sx={{ '&:last-child td': { border: 0 } }}>
                      <TableCell>
                        <Typography variant="body2" fontWeight="medium">
                          {vendedor.vendedorNombre}
                        </Typography>
                      </TableCell>
                      <TableCell align="center">
                        <Chip 
                          label={vendedor.totalVentas} 
                          size="small" 
                          color="secondary" 
                        />
                      </TableCell>
                      <TableCell align="right">
                        <Typography variant="body2" fontWeight="bold" color="primary">
                          S/ {vendedor.totalImporte?.toFixed(2)}
                        </Typography>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </TableContainer>
          </Paper>
        </Grid>

        {/* INVENTARIO BAJO STOCK */}
        <Grid item xs={12} lg={5}>
          <Paper sx={{ 
            p: 3, 
            height: '350px', 
            overflow: 'auto',
            textAlign: 'center',
            border: '2px solid #ffecb3',
            borderRadius: 3
          }}>
            <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center', mb: 3 }}>
              <Security sx={{ color: '#ff9800', mr: 1, fontSize: 28 }} />
              <Typography variant="h5" fontWeight="bold" color="#2c3e50">
                INVENTARIO BAJO STOCK
              </Typography>
            </Box>
            <TableContainer>
              <Table size="small">
                <TableHead>
                  <TableRow sx={{ background: '#fff3e0' }}>
                    <TableCell><strong>PRODUCTO</strong></TableCell>
                    <TableCell align="center"><strong>STOCK</strong></TableCell>
                    <TableCell align="right"><strong>PRECIO</strong></TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {inventarioBajo.map((producto) => (
                    <TableRow key={producto.id} hover sx={{ '&:last-child td': { border: 0 } }}>
                      <TableCell>
                        <Typography variant="body2" fontWeight="medium">
                          {producto.nombre}
                        </Typography>
                      </TableCell>
                      <TableCell align="center">
                        <Chip 
                          label={producto.stock} 
                          size="small" 
                          color="warning" 
                        />
                      </TableCell>
                      <TableCell align="right">
                        <Typography variant="body2" fontWeight="bold">
                          S/ {producto.precio?.toFixed(2)}
                        </Typography>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </TableContainer>
          </Paper>
        </Grid>

        {/* VENTAS ÚLTIMOS 7 DÍAS */}
        <Grid item xs={12} lg={5}>
          <Paper sx={{ 
            p: 3, 
            height: '350px', 
            overflow: 'auto',
            textAlign: 'center',
            border: '2px solid #e1f5fe',
            borderRadius: 3
          }}>
            <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center', mb: 3 }}>
              <BarChart sx={{ color: '#2196f3', mr: 1, fontSize: 28 }} />
              <Typography variant="h5" fontWeight="bold" color="#2c3e50">
                VENTAS ÚLTIMOS 7 DÍAS
              </Typography>
            </Box>
            <TableContainer>
              <Table size="small">
                <TableHead>
                  <TableRow sx={{ background: '#f5f5f5' }}>
                    <TableCell><strong>FECHA</strong></TableCell>
                    <TableCell align="center"><strong>VENTAS</strong></TableCell>
                    <TableCell align="right"><strong>TOTAL</strong></TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {ventas7Dias.map((dia, index) => (
                    <TableRow key={index} hover sx={{ '&:last-child td': { border: 0 } }}>
                      <TableCell>
                        <Typography variant="body2">
                          {dia.fecha}
                        </Typography>
                      </TableCell>
                      <TableCell align="center">
                        <Chip 
                          label={dia.cantidadVentas} 
                          size="small" 
                          color="info" 
                          variant="outlined"
                        />
                      </TableCell>
                      <TableCell align="right">
                        <Typography variant="body2" fontWeight="bold">
                          S/ {dia.totalVentas?.toFixed(2)}
                        </Typography>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </TableContainer>
          </Paper>
        </Grid>
      </Grid>
    </Container>
  );
};

export default Dashboard;