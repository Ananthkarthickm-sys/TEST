import axios from 'axios';

const API_BASE_URL = 'https://localhost:55546/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Frequency API
export const frequencyAPI = {
  getAll: () => api.get('/frequencies'),
  getById: (id) => api.get(`/frequencies/${id}`),
  create: (data) => api.post('/frequencies', data),
  update: (id, data) => api.put(`/frequencies/${id}`, data),
  delete: (id) => api.delete(`/frequencies/${id}`),
  getFirst: () => api.get('/frequencies/first'),
  getLast: () => api.get('/frequencies/last'),
  getPrevious: (id) => api.get(`/frequencies/${id}/previous`),
  getNext: (id) => api.get(`/frequencies/${id}/next`),
  getCount: () => api.get('/frequencies/count'),
  refresh: (id) => api.post(`/frequencies/${id}/refresh`),
};

// Loan API
export const loanAPI = {
  getAll: () => api.get('/loans'),
  getById: (id) => api.get(`/loans/${id}`),
  create: (data) => api.post('/loans', data),
  update: (id, data) => api.put(`/loans/${id}`, data),
  delete: (id) => api.delete(`/loans/${id}`),
  calculate: (data) => api.post('/loans/calculate', data),
  calculateFull: (id) => api.post(`/loans/${id}/calculate-full`),
  generateSchedule: (id) => api.post(`/loans/${id}/generate-schedule`),
};

// Payment Schedule API
export const paymentScheduleAPI = {
  getByLoan: (loanId) => api.get(`/paymentschedules/loan/${loanId}`),
  getById: (id) => api.get(`/paymentschedules/${id}`),
  getChartData: (loanId) => api.get(`/paymentschedules/loan/${loanId}/chart-data`),
  deleteByLoan: (loanId) => api.delete(`/paymentschedules/loan/${loanId}`),
};

// Payment Schedule API
export const attachmentAPI = {
  getByLoan: (loanId) => api.get(`/Attachments/getByLoan/${loanId}`),
  upload: (data) => api.post(`/Attachments/upload`, data, {
    headers: { "Content-Type": "multipart/form-data" }
  }),
};

export default api;
