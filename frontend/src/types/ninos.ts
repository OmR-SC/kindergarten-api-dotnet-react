export interface NinoReadDto {
  matricula: string;
  nombre: string;
  fechaNacimiento: string;
  fechaIngreso: string;
  fechaBaja: string | null;
  cedulaPagador: string;
}

export interface NinoCreateDto {
  nombre: string;
  fechaNacimiento: string;
  fechaIngreso: string;
  cedulaPagador: string;
}

export interface NinoUpdateDto {
  nombre: string;
  fechaNacimiento: string;
  fechaIngreso: string;
  fechaBaja?: string | null;
  cedulaPagador: string;
}
