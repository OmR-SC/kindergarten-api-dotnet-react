import axios from 'axios';
import type { PersonaReadDto, PersonaCreateDto, PersonaUpdateDto } from './types';

export const API_URL = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'http://localhost:5000',
});

export const api = axios.create({
  baseURL: `${API_URL}/api/v1`,
  headers: {
    'Content-Type': 'application/json',
  },
});

// ————— Personas —————

// Listar todas
export const getPersonas = (): Promise<PersonaReadDto[]> =>
  api.get('/personas').then((res) => res.data);

// Obtener una por cédula
export const getPersona = (cedula: string): Promise<PersonaReadDto> =>
  api.get(`/personas/${cedula}`).then((res) => res.data);

// Crear
export const createPersona = (dto: PersonaCreateDto): Promise<PersonaReadDto> =>
  api.post('/personas', dto).then((res) => res.data);

// Actualizar
export const updatePersona = (cedula: string, dto: PersonaUpdateDto) =>
  api.put(`/personas/${cedula}`, dto).then(() => {});

// Borrar
export const deletePersona = (cedula: string): Promise<void> =>
  api.delete(`/personas/${cedula}`).then(() => {});
