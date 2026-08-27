import axios from 'axios';

export const apiUrl = import.meta.env.VITE_API_URL ?? 'https://localhost:5001/api';

export const api = axios.create({ baseURL: apiUrl });

const TOKEN_KEY = 'smartbus_token';

api.interceptors.request.use((config) => {
  const token = localStorage.getItem(TOKEN_KEY);
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});
