import { api } from './axios';

import type { NinoReadDto, NinoCreateDto, NinoUpdateDto } from '../types/ninos';

// ————— NInos —————

// Listar todas
export const getNinos = (): Promise<NinoReadDto[]> =>
  api.get('/ninos').then((res) => {
    console.log('Data received from API:', res);
    return res.data;
  });

// Obtener una por matricula
export const getNino = (matricula: number): Promise<NinoReadDto> =>
  api.get(`/ninos/${matricula}`).then((res) => res.data);

// Crear
export const createNino = (dto: NinoCreateDto): Promise<NinoReadDto> =>
  api.post('/ninos', dto).then((res) => res.data);

// Actualizar
export const updateNino = (matricula: number, dto: NinoUpdateDto) =>
  api.put(`/ninos/${matricula}`, dto).then(() => {});

// Borrar
export const deleteNino = (matricula: string): Promise<void> =>
  api.delete(`/ninos/${matricula}`).then(() => {});