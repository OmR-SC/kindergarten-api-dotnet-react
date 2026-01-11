import type { NinoCreateDto, NinoReadDto, NinoUpdateDto } from 'src/types/ninos';

import axios from 'axios';
import dayjs, { Dayjs } from 'dayjs';
import { useEffect, useRef, useState } from 'react';

import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
} from '@mui/material';

import { createNino, updateNino } from 'src/api/ninos';

//import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
//import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';

type Props = {
  open: boolean;
  onClose: () => void;
  onCreated: (newNino: any) => void;
  initial?: NinoReadDto;
};

type FormErrors = Partial<Record<keyof NinoReadDto, string>> & {
  __global?: string;
};

export function NinoFormDialog({ open, onClose, onCreated, initial }: Props) {
  const initialForm: NinoReadDto = {
    matricula: '',
    nombre: '',
    fechaNacimiento: '',
    fechaIngreso: '',
    fechaBaja: '',
    cedulaPagador: '',
  };
  const [form, setForm] = useState<NinoReadDto>({
    matricula: '',
    nombre: '',
    fechaNacimiento: '',
    fechaIngreso: '',
    fechaBaja: '',
    cedulaPagador: '',
  });

  const [errors, setErrors] = useState<FormErrors>({});

  const [saving, setSaving] = useState(false);

  const [snackbarOpen, setSnackbarOpen] = useState(false);

  const [snackbarMessage, setSnackbarMessage] = useState('');

  const isEditMode = !!initial;

  // Refs para enfoque dinámico
  // const cedulaRef = useRef<HTMLInputElement>(null);
  const nombreRef = useRef<HTMLInputElement>(null);

  useEffect(() => {
    // Reseteamos el formulario y errores al abrir el diálogo
    if (!open) return;

    if (initial) {
      setForm({
        matricula: initial.matricula,
        nombre: initial.nombre,
        fechaNacimiento: initial.fechaNacimiento,
        fechaIngreso: initial.fechaIngreso,
        fechaBaja: initial.fechaBaja || '',
        cedulaPagador: initial.cedulaPagador,
      });

      // Enfocar campo nombre al editar
      setTimeout(() => nombreRef.current?.focus(), 100);
    } else {
      setForm(initialForm);

      // Enfocar campo cédula al crear
      //setTimeout(() => nombreRef.current?.focus(), 100);
    }
    // Enfocar campo nombre al abrir
    setTimeout(() => nombreRef.current?.focus(), 100);

    setErrors({});
  }, [open]);

  const handleChange = (field: keyof NinoReadDto) => (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    // 1) Actualiza el form
    setForm((f) => ({ ...f, [field]: value }));

    // 2) Limpia el posible error de ese campo
    setErrors((prev) => ({ ...prev, [field]: undefined }));
  };

  const handleSubmit = async () => {
    setSaving(true);
    setErrors({});
    try {
      const payload = {
        ...form,
        fechaNacimiento: form.fechaNacimiento === '' ? null : form.fechaNacimiento,
        fechaIngreso: form.fechaIngreso === '' ? null : form.fechaIngreso,
        fechaBaja: form.fechaBaja === '' ? null : form.fechaBaja,
      };

      let result;

      if (isEditMode && initial) {
        await updateNino(parseInt(initial.matricula), payload);
        result = payload;
        setSnackbarMessage('Nino actualizado correctamente');
      } else {
        result = await createNino(payload);
        setSnackbarMessage('Nino creado correctamente');
      }

      setSnackbarOpen(true);
      onCreated(result);
      // Cerrar diálogo tras pequeño delay para que se vea el snackbar
      setTimeout(() => {
        setSnackbarOpen(false);
        onClose();
      }, 1800);
    } catch (error: any) {
            console.error('Error en submit:', error);
      if (axios.isAxiosError(error)) {
        const response = error.response;

        if (!response) {
          // Error de red (como servidor apagado)
          setErrors({ __global: 'No se pudo conectar con el servidor. Intente más tarde.' });
          return;
        }

        console.error('Error en submit:', response.data.errors);

        if (response.status === 400 && response.data.errors) {
          const validationErrors = response.data.errors;
          const formattedErrors: FormErrors = {};

          // --- AQUÍ ESTÁ LA MAGIA CORREGIDA ---
          // Obtenemos las claves reales de tu formulario (ej: ['nombre', 'cedulaPagador'...])
          const formKeys = Object.keys(form);

          for (const serverKey in validationErrors) {
            // Buscamos en tus claves locales cuál coincide ignorando mayúsculas
            const matchingKey = formKeys.find(
              (fk) => fk.toLowerCase() === serverKey.toLowerCase()
            ) as keyof NinoReadDto | undefined;

            if (matchingKey) {
              // Asignamos el error a la clave CORRECTA del frontend (camelCase)
              formattedErrors[matchingKey] = validationErrors[serverKey][0];
            }
          }

          // Si después de mapear no encontramos errores específicos (ej: error de lógica global),
          // mostramos un error genérico, si no, mostramos los errores de campo.
          if (Object.keys(formattedErrors).length > 0) {
            setErrors(formattedErrors);
          } else {
            // A veces el error viene en response.data.title o similar si no es de campos
            setErrors({ __global: 'Error de validación, verifique los datos.' });
          }

          // for (const key in validationErrors) {
          //   // tomamos sólo el primer mensaje de cada campo
          //   const lc = key.toLowerCase() as keyof NinoReadDto;

          //   if (lc in form) {
          //     formattedErrors[lc] = validationErrors[key][0];
          //   }
          // }
          setErrors(formattedErrors);
        } else {
          setErrors({ __global: 'Ha ocurrido un error inesperado.' });
        }
      } else {
        setErrors({ __global: 'Error desconocido al crear nino.' });
      }
    } finally {
      setSaving(false);
    }
  };

  return (
    <>
      <Dialog
        open={open}
        onClose={onClose}
        slotProps={{
          transition: {
            onExited: () => {
              setForm(initialForm);
              setErrors({});
            },
          },
        }}
        fullWidth
        maxWidth="sm"
        disableRestoreFocus
      >
        <DialogTitle>{isEditMode ? 'Editar Nino' : 'Agregar Nino'}</DialogTitle>
        <DialogContent>
          {/* Mensaje de error general si existe */}
          {errors.__global && (
            <Alert severity="error" sx={{ mb: 2 }}>
              {errors.__global}
            </Alert>
          )}

          <form
            onSubmit={(e) => {
              e.preventDefault();
              handleSubmit();
            }}
          >
            <Stack spacing={2} mt={1}>
              {isEditMode && (
                <TextField
                  label="Matricula"
                  value={form.matricula}
                  onChange={handleChange('matricula')}
                  fullWidth
                  disabled={isEditMode} // Deshabilitar si es edición
                  error={!!errors.matricula}
                  helperText={errors.matricula}
                  //inputRef={nombreRef} // Enfocar al crear
                />
              )}

              <TextField
                label="Nombre"
                value={form.nombre}
                onChange={handleChange('nombre')}
                fullWidth
                error={!!errors.nombre}
                helperText={errors.nombre}
                inputRef={nombreRef} // Enfocar al crear
              />
              {/* <TextField
                label="Fecha de Nacimiento"
                value={form.fechaNacimiento}
                onChange={handleChange('fechaNacimiento')}
                fullWidth
                error={!!errors.fechaNacimiento}
                helperText={errors.fechaNacimiento}
              />
              <TextField
                label="Fecha de Ingreso"
                value={form.fechaIngreso}
                onChange={handleChange('fechaIngreso')}
                fullWidth
                error={!!errors.fechaIngreso}
                helperText={errors.fechaIngreso}
              />
              {isEditMode && (
                <TextField
                  label="Fecha de Baja"
                  value={form.fechaBaja}
                  onChange={handleChange('fechaBaja')}
                  fullWidth
                  error={!!errors.fechaBaja}
                  helperText={errors.fechaBaja}
                />
              )} */}
              <TextField
                label="Cedula Pagador"
                value={form.cedulaPagador}
                onChange={handleChange('cedulaPagador')}
                fullWidth
                error={!!errors.cedulaPagador}
                helperText={errors.cedulaPagador}
              />

              {/* Fecha de Nacimiento - DatePicker */}
              <DatePicker
                label="Fecha de Nacimiento"
                value={form.fechaNacimiento ? dayjs(form.fechaNacimiento) : null}
                format="DD/MM/YYYY"
                onChange={(newValue: Dayjs | null) => {
                  const newStr = newValue ? newValue.format('YYYY-MM-DD') : '';
                  setForm((f) => ({ ...f, fechaNacimiento: newStr }));
                  setErrors((prev) => ({ ...prev, fechaNacimiento: undefined }));
                }}
                slotProps={{
                  textField: {
                    fullWidth: true,
                    error: !!errors.fechaNacimiento,
                    helperText: errors.fechaNacimiento,
                  },
                }}
              />

              {/* Fecha de Ingreso - DatePicker */}
              <DatePicker
                label="Fecha de Ingreso"
                value={form.fechaIngreso ? dayjs(form.fechaIngreso) : null}
                format="DD/MM/YYYY"
                onChange={(newValue) => {
                  const newStr = newValue ? newValue.format('YYYY-MM-DD') : '';
                  setForm((f) => ({ ...f, fechaIngreso: newStr }));
                  setErrors((prev) => ({ ...prev, fechaIngreso: undefined }));
                }}
                slotProps={{
                  textField: {
                    fullWidth: true,
                    error: !!errors.fechaIngreso,
                    helperText: errors.fechaIngreso,
                  },
                }}
              />

              {isEditMode && (
                <DatePicker
                  label="Fecha de Baja"
                  value={form.fechaBaja ? dayjs(form.fechaBaja) : null}
                  format="DD/MM/YYYY"
                  onChange={(newValue: Dayjs | null) => {
                    const newStr = newValue ? newValue.format('YYYY-MM-DD') : '';
                    setForm((f) => ({ ...f, fechaBaja: newStr }));
                    setErrors((prev) => ({ ...prev, fechaBaja: undefined }));
                  }}
                  slotProps={{
                    textField: {
                      fullWidth: true,
                      error: !!errors.fechaBaja,
                      helperText: errors.fechaBaja,
                    },
                  }}
                />
              )}
            </Stack>

            <DialogActions sx={{ pr: 3, pb: 2 }}>
              <Button onClick={onClose} disabled={saving}>
                Cancelar
              </Button>
              <Button type="submit" variant="contained" disabled={saving}>
                {saving ? 'Guardando…' : isEditMode ? 'Guardar cambios' : 'Crear'}
              </Button>
            </DialogActions>
          </form>
        </DialogContent>
      </Dialog>

      <Snackbar
        open={snackbarOpen}
        autoHideDuration={2000}
        onClose={() => setSnackbarOpen(false)}
        anchorOrigin={{ vertical: 'top', horizontal: 'right' }}
      >
        <Alert severity="success" sx={{ width: '100%' }}>
          {snackbarMessage}
        </Alert>
      </Snackbar>
    </>
  );
}
