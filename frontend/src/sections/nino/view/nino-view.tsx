import type { NinoReadDto } from 'src/types/ninos';

import { useState, useEffect, useCallback } from 'react';

import Box from '@mui/material/Box';
import Card from '@mui/material/Card';
import Table from '@mui/material/Table';
import Button from '@mui/material/Button';
import { Alert, Snackbar } from '@mui/material';
import TableBody from '@mui/material/TableBody';
import Typography from '@mui/material/Typography';
import TableContainer from '@mui/material/TableContainer';
import TablePagination from '@mui/material/TablePagination';

//import { _users } from 'src/_mock';
import { getNinos, deleteNino } from 'src/api/ninos';
import { DashboardContent } from 'src/layouts/dashboard';

import { Iconify } from 'src/components/iconify';
import { Scrollbar } from 'src/components/scrollbar';

import { TableNoData } from '../table-no-data';
import { NinoTableRow } from '../nino-table-row';
import { NinoTableHead } from '../nino-table-head';
import { TableEmptyRows } from '../table-empty-rows';
import { NinoTableToolbar } from '../nino-table-toolbar';
import { ConfirmDialog } from '../components/ConfirmDialog';
import { NinoFormDialog } from '../components/NinoFormDialog';
import { emptyRows, applyFilter, getComparator } from '../utils';

import type { NinoProps } from '../nino-table-row';

// ----------------------------------------------------------------------

export function NinoView() {
  const table = useTable();

  const [filterName, setFilterName] = useState('');

  const [ninos, setNinos] = useState<NinoReadDto[]>([]);

  const [loading, setLoading] = useState(true);

  const [snackbarOpen, setSnackbarOpen] = useState(false);

  const [snackbarMessage, setSnackbarMessage] = useState('');

  //Edit Nino
  const [editNino, setEditNino] = useState<NinoReadDto | null>(null);

  // Dialog para crear un nuevo Nino o editar uno existente
  const [openDialog, setOpenDialog] = useState(false);

  // Abrir diálogo para crear (editNino = null)
  function handleOpenCreate() {
    setEditNino(null);
    setOpenDialog(true);
  }

  // Abrir diálogo para editar (editNino = nino a editar)
  function handleOpenEdit(nino: NinoReadDto) {
    setEditNino(nino);
    setOpenDialog(true);
  }

  // Cerrar diálogo
  function handleCloseDialog() {
    setOpenDialog(false);
    setEditNino(null);
  }

  const handleCreated = (newNino: NinoReadDto) => {
    // recarga o inserta en la lista local
    setNinos((prev) => [newNino, ...prev]);
    table.onResetPage();
    handleCloseDialog();
  };

  function handleUpdated(updatedNino: NinoReadDto) {
    // refrescar lista o hacer map sobre ninos para actualizar la fila
    setNinos((arr) => arr.map((n) => (n.matricula === updatedNino.matricula ? updatedNino : n)));
    handleCloseDialog();
  }

  //Delete
  const [ninoToDelete, setNinoToDelete] = useState<NinoReadDto | null>(null);

  const handleDeleteConfirm = async () => {
    if (!ninoToDelete) return;

    try {
      await deleteNino(ninoToDelete.matricula);
      setNinos((prev) => prev.filter((n) => n.matricula !== ninoToDelete.matricula));
      setSnackbarMessage('Nino deleted successfully');
      setSnackbarOpen(true);
    } catch (err) {
      console.error('Error deleting:', err);
      alert('Error deleting the nino.');
    } finally {
      setNinoToDelete(null);
      setTimeout(() => {
        setSnackbarOpen(false);
      }, 1800);
    }
  };

  useEffect(() => {
    const fecthData = async () => {
      setLoading(true);
      try {
        const data = await getNinos();
        setNinos(data);
      } catch (error) {
        console.error('Error loading ninos:', error);
      } finally {
        setLoading(false);
      }
    };
    fecthData();
  }, []);

  const dataFiltered = applyFilter({
    inputData: mapDtoToProps(ninos),
    comparator: getComparator(table.order, table.orderBy),
    filterName,
  });

  const notFound = !dataFiltered.length && !!filterName;

  return (
    <>
      <DashboardContent>
        <Box
          sx={{
            mb: 5,
            display: 'flex',
            alignItems: 'center',
          }}
        >
          <Typography variant="h4" sx={{ flexGrow: 1 }}>
            Ninos
          </Typography>
          <Button
            variant="contained"
            color="inherit"
            startIcon={<Iconify icon="mingcute:add-line" />}
            onClick={handleOpenCreate}
          >
            Add Nino
          </Button>
        </Box>

        <NinoFormDialog
          open={openDialog}
          initial={editNino ?? undefined}
          onClose={handleCloseDialog}
          onCreated={editNino ? handleUpdated : handleCreated}
        />

        <Card>
          <NinoTableToolbar
            numSelected={table.selected.length}
            filterName={filterName}
            onFilterName={(event: React.ChangeEvent<HTMLInputElement>) => {
              setFilterName(event.target.value);
              table.onResetPage();
            }}
          />

          <Scrollbar>
            <TableContainer sx={{ overflow: 'unset' }}>
              <Table sx={{ minWidth: 800 }}>
                <NinoTableHead
                  order={table.order}
                  orderBy={table.orderBy}
                  rowCount={ninos.length}
                  numSelected={table.selected.length}
                  onSort={table.onSort}
                  onSelectAllRows={(checked) =>
                    table.onSelectAllRows(
                      checked,
                      ninos.map((user) => user.matricula.toString())
                    )
                  }
                  headLabel={[
                    { id: 'nombre', label: 'Nombre' },
                    { id: 'matricula', label: 'Matricula' },
                    { id: 'fechaNacimiento', label: 'Fecha Nacimiento' },
                    { id: 'fechaIngreso', label: 'Fecha Ingreso' },
                    { id: 'fechaBaja', label: 'Fecha Baja' },
                    { id: 'cedulaPagador', label: 'Cédula Pagador' },
                    { id: '' },
                  ]}
                />
                <TableBody>
                  {dataFiltered
                    .slice(
                      table.page * table.rowsPerPage,
                      table.page * table.rowsPerPage + table.rowsPerPage
                    )
                    .map((row) => (
                      <NinoTableRow
                        key={row.matricula}
                        row={row}
                        selected={table.selected.includes(String(row.matricula))}
                        onSelectRow={() => table.onSelectRow(String(row.matricula))}
                        onEdit={(p) => handleOpenEdit(p)}
                        onDelete={(p) => setNinoToDelete(p)}
                      />
                    ))}

                  <TableEmptyRows
                    height={68}
                    emptyRows={emptyRows(table.page, table.rowsPerPage, ninos.length)}
                  />

                  {notFound && <TableNoData searchQuery={filterName} />}
                </TableBody>
              </Table>
            </TableContainer>
          </Scrollbar>

          <TablePagination
            component="div"
            page={table.page}
            count={ninos.length}
            rowsPerPage={table.rowsPerPage}
            onPageChange={table.onChangePage}
            rowsPerPageOptions={[5, 10, 25]}
            onRowsPerPageChange={table.onChangeRowsPerPage}
          />
        </Card>
      </DashboardContent>
      <ConfirmDialog
        open={!!ninoToDelete}
        content={`Are you sure you want to delete ${ninoToDelete?.nombre}?`}
        onClose={() => setNinoToDelete(null)}
        onConfirm={handleDeleteConfirm}
      />
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

// ----------------------------------------------------------------------

export function useTable() {
  const [page, setPage] = useState(0);
  const [orderBy, setOrderBy] = useState('nombre');
  const [rowsPerPage, setRowsPerPage] = useState(5);
  const [selected, setSelected] = useState<string[]>([]);
  const [order, setOrder] = useState<'asc' | 'desc'>('asc');

  const onSort = useCallback(
    (id: string) => {
      const isAsc = orderBy === id && order === 'asc';
      setOrder(isAsc ? 'desc' : 'asc');
      setOrderBy(id);
    },
    [order, orderBy]
  );

  const onSelectAllRows = useCallback((checked: boolean, newSelecteds: string[]) => {
    if (checked) {
      setSelected(newSelecteds);
      return;
    }
    setSelected([]);
  }, []);

  const onSelectRow = useCallback(
    (inputValue: string) => {
      const newSelected = selected.includes(inputValue)
        ? selected.filter((value) => value !== inputValue)
        : [...selected, inputValue];

      setSelected(newSelected);
    },
    [selected]
  );

  const onResetPage = useCallback(() => {
    setPage(0);
  }, []);

  const onChangePage = useCallback((event: unknown, newPage: number) => {
    setPage(newPage);
  }, []);

  const onChangeRowsPerPage = useCallback(
    (event: React.ChangeEvent<HTMLInputElement>) => {
      setRowsPerPage(parseInt(event.target.value, 10));
      onResetPage();
    },
    [onResetPage]
  );

  return {
    page,
    order,
    onSort,
    orderBy,
    selected,
    rowsPerPage,
    onSelectRow,
    onResetPage,
    onChangePage,
    onSelectAllRows,
    onChangeRowsPerPage,
  };
}

function mapDtoToProps(data: NinoReadDto[]): NinoProps[] {
  if (!Array.isArray(data)) {
    console.warn('mapDtoToProps recibió datos no válidos:', data);
    return [];
  }
  return data.map((n, index: number) => ({
    ...n,
    /*
      nombre: n.nombre,
      fechaNacimiento: n.fechaNacimiento,
      direccion: n.fechaIngreso,
      telefono: n.cedulaPagador,
      */
    avatarUrl: `/assets/images/avatar/avatar-${index + 1}.webp`, // o lógica para obtener la URL del avatar
  }));
}
