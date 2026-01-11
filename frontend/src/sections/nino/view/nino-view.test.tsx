// @vitest-environment jsdom
import '@testing-library/jest-dom/vitest';

import { it, describe, vi, expect, afterEach, beforeEach } from 'vitest';
import { render, screen, waitFor, cleanup, fireEvent } from '@testing-library/react';

import * as ninosApi from 'src/api/ninos';

import { NinoView } from './nino-view';

// ----------------------------------------------------------------------

vi.mock('src/api/ninos', () => ({
  getNinos: vi.fn(),
  deleteNino: vi.fn(),
}));

vi.mock('src/components/iconify', () => ({
  Iconify: () => <span data-testid="icon-mock" />,
}));

vi.mock('../components/NinoFormDialog', () => ({
  NinoFormDialog: ({ open }: { open: boolean }) =>
    open ? <div role="dialog">Mock Dialog Content</div> : null,
}));

const mockNinos = [
  {
    matricula: 101,
    nombre: 'Pepito Perez',
    fechaNacimiento: '2020-05-15',
    fechaIngreso: '2024-01-10',
    fechaBaja: null,
    cedulaPagador: '001-0000000-1',
  },
  {
    matricula: 102,
    nombre: 'Maria Lopez',
    fechaNacimiento: '2021-02-20',
    fechaIngreso: '2024-02-01',
    fechaBaja: null,
    cedulaPagador: '002-0000000-2',
  },
];

describe('NinoView Component', () => {
  beforeEach(() => {
    vi.resetAllMocks();
  });

  afterEach(() => {
    cleanup();
  });

  it('should render the ninos list on load', async () => {
    (ninosApi.getNinos as any).mockResolvedValue(mockNinos);

    render(<NinoView />);

    expect(screen.getByText('Ninos')).toBeInTheDocument();

    await waitFor(() => {
      expect(screen.getByText('Pepito Perez')).toBeInTheDocument();
      expect(screen.getByText('Maria Lopez')).toBeInTheDocument();
    });
  });

  it('should render correctly when the ninos list is empty', async () => {
    (ninosApi.getNinos as any).mockResolvedValue([]);

    render(<NinoView />);

    await waitFor(() => {
      expect(screen.queryByText('Pepito Perez')).not.toBeInTheDocument();
    });
  });

  it('should open the create dialog when clicking the "Add Nino" button', async () => {
    (ninosApi.getNinos as any).mockResolvedValue([]);
    render(<NinoView />);

    const addButtons = screen.getAllByRole('button', { name: /add nino/i });
    fireEvent.click(addButtons[0]);

    await waitFor(() => {
      const dialog = screen.getByRole('dialog');
      expect(dialog).toBeInTheDocument();
      expect(screen.getByText('Mock Dialog Content')).toBeInTheDocument();
    });
  });
});
