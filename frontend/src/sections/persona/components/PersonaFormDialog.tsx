import type { PersonaCreateDto } from 'src/types/persona';

import axios, { AxiosError } from 'axios';
import { useEffect, useState } from 'react';

import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  Stack,
  Alert,
  Snackbar,
} from '@mui/material';

import { createPersona } from 'src/api/persona';

type Props = {
  open: boolean;
  onClose: () => void;
  onCreated: (newPersona: any) => void;
};

type FormErrors = Partial<Record<keyof PersonaCreateDto, string>> & {
  __global?: string;
};

export function PersonaFormDialog({ open, onClose, onCreated }: Props) {
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

  useEffect(() => {
    // Reseteamos el formulario y errores al abrir el diálogo
    if (!open) {
      setErrors({});
      setForm({ cedula: '', nombre: '', telefono: '', direccion: '' });
    }
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
    try {
      const created = await createPersona(form);
      setSnackbarOpen(true);
      onCreated(created);
      // Cerrar diálogo tras pequeño delay para que se vea el snackbar
      setTimeout(() => {
        setSnackbarOpen(false);
        onClose();
      }, 1800);
      onClose();
    } catch (error: any) {
      console.error('Error creating persona:', error);
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

          for (const key in validationErrors) {
            // tomamos sólo el primer mensaje de cada campo
            const lc = key.toLowerCase() as keyof PersonaCreateDto;

            if (lc in form) {
              formattedErrors[lc] = validationErrors[key][0];
            }
          }
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
        <DialogTitle>Crear nueva Persona</DialogTitle>
        <DialogContent>
          {/* Mensaje de error general si existe */}
          {errors.__global && (
            <Alert severity="error" sx={{ mb: 2 }}>
              {errors.__global}
            </Alert>
          )}

          <Stack spacing={2} mt={1}>
            <TextField
              autoFocus
              label="Cédula"
              value={form.cedula}
              onChange={handleChange('cedula')}
              fullWidth
              error={!!errors.cedula}
              helperText={errors.cedula}
            />
            <TextField
              label="Nombre"
              value={form.nombre}
              onChange={handleChange('nombre')}
              fullWidth
              error={!!errors.nombre}
              helperText={errors.nombre}
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
        </DialogContent>
        <DialogActions sx={{ pr: 3, pb: 2 }}>
          <Button onClick={onClose} disabled={saving}>
            Cancelar
          </Button>
          <Button variant="contained" onClick={handleSubmit} disabled={saving}>
            {saving ? 'Guardando…' : 'Crear'}
          </Button>
        </DialogActions>
      </Dialog>

      {/* Snackbar de éxito */}
      <Snackbar
        open={snackbarOpen}
        autoHideDuration={2000}
        onClose={() => setSnackbarOpen(false)}
        anchorOrigin={{ vertical: 'top', horizontal: 'right' }}
      >
        <Alert severity="success" sx={{ width: '100%' }}>
          Persona creada correctamente
        </Alert>
      </Snackbar>
    </>
  );
}
