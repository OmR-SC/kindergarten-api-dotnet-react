// @vitest-environment jsdom
import '@testing-library/jest-dom/vitest';
import { render, screen, waitFor, fireEvent, cleanup } from '@testing-library/react';
import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import { PersonaView } from './persona-view';
import * as personaApi from 'src/api/persona';

// ----------------------------------------------------------------------

// 1. Mockeamos la API de Persona
vi.mock('src/api/persona', () => ({
  getPersonas: vi.fn(),
  deletePersona: vi.fn(),
}));

// 2. Mockeamos componentes visuales para evitar errores de contexto
vi.mock('src/components/iconify', () => ({
  Iconify: () => <span data-testid="icon-mock" />,
}));

// 3. Mockeamos el Dialog de Persona para evitar problemas con validaciones/formularios complejos en este test
vi.mock('../components/PersonaFormDialog', () => ({
  PersonaFormDialog: ({ open }: { open: boolean }) => 
    open ? <div role="dialog">Mock Persona Dialog</div> : null
}));

// Datos de prueba (Dummy Data)
const mockPersonas = [
  {
    cedula: '001-0000000-1',
    nombre: 'Juan Perez',
    telefono: '809-555-0101',
    direccion: 'Calle Falsa 123',
  },
  {
    cedula: '002-0000000-2',
    nombre: 'Ana Gomez',
    telefono: '809-555-0102',
    direccion: 'Av. Siempre Viva 742',
  },
];

describe('PersonaView Component', () => {
  // Limpiamos los espías (mocks) antes de cada test
  beforeEach(() => {
    vi.resetAllMocks();
  });

  // Limpiamos el DOM después de cada test (Evita el error de elementos fantasmas)
  afterEach(() => {
    cleanup();
  });

  it('should render the personas list on load', async () => {
    // Simulamos que la API devuelve datos
    (personaApi.getPersonas as any).mockResolvedValue(mockPersonas);

    render(<PersonaView />);

    // Verificamos el título principal
    expect(screen.getByText('Personas')).toBeInTheDocument();

    // Esperamos a que la tabla se llene
    await waitFor(() => {
      expect(screen.getByText('Juan Perez')).toBeInTheDocument();
      expect(screen.getByText('Ana Gomez')).toBeInTheDocument();
      // Verificamos también un dato extra como la cédula
      expect(screen.getByText('001-0000000-1')).toBeInTheDocument();
    });
  });

  it('should render correctly when the personas list is empty', async () => {
    // Simulamos que la API devuelve lista vacía
    (personaApi.getPersonas as any).mockResolvedValue([]);

    render(<PersonaView />);

    // Esperamos y verificamos que NO esté Juan Perez
    await waitFor(() => {
      expect(screen.queryByText('Juan Perez')).not.toBeInTheDocument();
    });
  });

  it('should open the create dialog when clicking the "Add Persona" button', async () => {
    (personaApi.getPersonas as any).mockResolvedValue([]);
    render(<PersonaView />);

    // Buscamos todos los botones y hacemos click en el de agregar
    const addButtons = screen.getAllByRole('button', { name: /add persona/i });
    fireEvent.click(addButtons[0]);

    // Verificamos que aparezca nuestro Mock del diálogo
    await waitFor(() => {
      expect(screen.getByText('Mock Persona Dialog')).toBeInTheDocument();
    });
  });
});