import type { PersonaReadDto, PersonaCreateDto } from 'src/types/persona';

import axios from 'axios';
import { useRef, useState, useEffect } from 'react';

import {
  Stack,
  Alert,
  Dialog,
  Button,
  Snackbar,
  TextField,
  DialogTitle,
  DialogContent,
  DialogActions,
} from '@mui/material';

import { createPersona, updatePersona } from 'src/api/persona';

type Props = {
  open: boolean;
  onClose: () => void;
  onCreated: (newPersona: any) => void;
  initial?: PersonaReadDto;
};

type FormErrors = Partial<Record<keyof PersonaCreateDto, string>> & {
  __global?: string;
};

export function PersonaFormDialog({ open, onClose, onCreated, initial }: Props) {
  const initialForm: PersonaCreateDto = { cedula: '', nombre: '', telefono: '', direccion: '' };
  const [form, setForm] = useState<PersonaCreateDto>({
    cedula: '',
    nombre: '',
    telefono: '',
    direccion: '',
  });

  const [errors, setErrors] = useState<FormErrors>({});

  const [saving, setSaving] = useState(false);

  const [snackbarOpen, setSnackbarOpen] = useState(false);

  const [snackbarMessage, setSnackbarMessage] = useState('');

  const isEditMode = !!initial;

  // Refs para enfoque dinámico
  const cedulaRef = useRef<HTMLInputElement>(null);
  const nombreRef = useRef<HTMLInputElement>(null);

  useEffect(() => {
    // Reseteamos el formulario y errores al abrir el diálogo
    if (!open) return;

    if (initial) {
      setForm({
        cedula: initial.cedula,
        nombre: initial.nombre,
        telefono: initial.telefono,
        direccion: initial.direccion,
      });

      // Enfocar campo nombre al editar
      setTimeout(() => nombreRef.current?.focus(), 100);
    } else {
      setForm(initialForm);

      // Enfocar campo cédula al crear
      setTimeout(() => cedulaRef.current?.focus(), 100);
    }
    setErrors({});
  }, [open]);

  const handleChange =
    (field: keyof PersonaCreateDto) => (e: React.ChangeEvent<HTMLInputElement>) => {
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
      let result;

      if (isEditMode && initial) {
        await updatePersona(initial.cedula, form);
        result = form;
        setSnackbarMessage('Persona actualizada correctamente');
      } else {
        result = await createPersona(form);
        setSnackbarMessage('Persona creada correctamente');
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

        if (response.status === 400 && response.data.errors) {
          const validationErrors = response.data.errors;
          const formattedErrors: FormErrors = {};

          const formKeys = Object.keys(form);

          for (const serverKey in validationErrors) {
            // Buscamos en tus claves locales cuál coincide ignorando mayúsculas
            const matchingKey = formKeys.find(
              (fk) => fk.toLowerCase() === serverKey.toLowerCase()
            ) as keyof PersonaCreateDto | undefined;

            if (matchingKey) {
              formattedErrors[matchingKey] = validationErrors[serverKey][0];
            }
          }

          if (Object.keys(formattedErrors).length > 0) {
            setErrors(formattedErrors);
          } else {
            setErrors({ __global: 'Error de validación, verifique los datos.' });
          }

          // for (const key in validationErrors) {
          //   // tomamos sólo el primer mensaje de cada campo
          //   const lc = key.toLowerCase() as keyof PersonaCreateDto;

          //   if (lc in form) {
          //     formattedErrors[lc] = validationErrors[key][0];
          //   }
          // }
          setErrors(formattedErrors);
        } else {
          setErrors({ __global: 'Ha ocurrido un error inesperado.' });
        }
      } else {
        setErrors({ __global: 'Error desconocido al crear persona.' });
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
        <DialogTitle>{isEditMode ? 'Editar Persona' : 'Crear nueva Persona'}</DialogTitle>
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
              <TextField
                label="Cédula"
                value={form.cedula}
                onChange={handleChange('cedula')}
                fullWidth
                disabled={isEditMode} // Deshabilitar si es edición
                error={!!errors.cedula}
                helperText={errors.cedula}
                inputRef={cedulaRef} // Enfocar al crear
              />
              <TextField
                label="Nombre"
                value={form.nombre}
                onChange={handleChange('nombre')}
                fullWidth
                error={!!errors.nombre}
                helperText={errors.nombre}
                inputRef={nombreRef} // Enfocar al editar
              />
              <TextField
                label="Teléfono"
                value={form.telefono}
                onChange={handleChange('telefono')}
                fullWidth
                error={!!errors.telefono}
                helperText={errors.telefono}
              />
              <TextField
                label="Dirección"
                value={form.direccion}
                onChange={handleChange('direccion')}
                fullWidth
                error={!!errors.direccion}
                helperText={errors.direccion}
              />
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
