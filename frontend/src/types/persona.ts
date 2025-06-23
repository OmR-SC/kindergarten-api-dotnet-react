export interface PersonaReadDto {
  cedula: string;
  nombre: string;
  telefono: string;
  direccion: string;
}

export interface PersonaCreateDto {
  cedula: string;
  nombre: string;
  telefono: string;
  direccion: string;
}

export interface PersonaUpdateDto {
  nombre: string;
  telefono: string;
  direccion: string;
}
