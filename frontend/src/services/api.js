import axios from 'axios';

const API_BASE_URL = 'http://localhost:5216/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

export const authAPI = {
  login: (credentials) => api.post('/Usuarios/login', credentials),
};
export const dashboardAPI = {
  getEstadisticas: () => api.get('/Dashboard/estadisticas'),
  getVentasUltimos7Dias: () => api.get('/Dashboard/ventas-ultimos-7-dias'),
  getProductosMasVendidos: () => api.get('/Dashboard/productos-mas-vendidos'),
};
export const reportesAPI = {
  getVentasPorFecha: (fechaInicio, fechaFin) => 
    api.get(`/Reportes/ventas-por-fecha?fechaInicio=${fechaInicio}&fechaFin=${fechaFin}`),
  getInventarioBajoStock: () => api.get('/Reportes/inventario-bajo-stock'),
  getVentasPorVendedor: () => api.get('/Reportes/ventas-por-vendedor'),
};
export default api;