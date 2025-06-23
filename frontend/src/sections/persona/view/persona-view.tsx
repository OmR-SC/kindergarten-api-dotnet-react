import { useState, useCallback, useEffect } from 'react';

import Box from '@mui/material/Box';
import Card from '@mui/material/Card';
import Table from '@mui/material/Table';
import Button from '@mui/material/Button';
import TableBody from '@mui/material/TableBody';
import Typography from '@mui/material/Typography';
import TableContainer from '@mui/material/TableContainer';
import TablePagination from '@mui/material/TablePagination';

import { _users } from 'src/_mock';
import { getPersonas } from 'src/api/persona';
import { DashboardContent } from 'src/layouts/dashboard';

import { Iconify } from 'src/components/iconify';
import { Scrollbar } from 'src/components/scrollbar';

import { PersonaCreateDto, PersonaReadDto } from 'src/types/persona';

import { TableNoData } from '../table-no-data';
import { TableEmptyRows } from '../table-empty-rows';
import { PersonaTableRow } from '../persona-table-row';
import { PersonaTableHead } from '../persona-table-head';
import { PersonaTableToolbar } from '../persona-table-toolbar';
import { emptyRows, applyFilter, getComparator } from '../utils';
import { PersonaFormDialog } from '../components/PersonaFormDialog';

import type { PersonaProps } from '../persona-table-row';

// ----------------------------------------------------------------------

export function PersonaView() {
  const table = useTable();

  const [filterName, setFilterName] = useState('');

  const [personas, setPersonas] = useState<PersonaReadDto[]>([]);

  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fecthData = async () => {
      setLoading(true);
      try {
        const data = await getPersonas();
        setPersonas(data);
      } catch (error) {
        console.error('Error loading personas:', error);
      } finally {
        setLoading(false);
      }
    };
    fecthData();
  }, []);

  // Dialog para crear una nueva persona
  const [openDialog, setOpenDialog] = useState(false);

  const handleCreated = (newPersona: PersonaCreateDto) => {
    // recarga o inserta en la lista local
    setPersonas((prev) => [newPersona, ...prev]);
    table.onResetPage();
  };

  function mapDtoToProps(pers: PersonaReadDto[]): PersonaProps[] {
    if (!Array.isArray(pers)) {
      console.warn('mapDtoToProps recibió datos no válidos:', pers);
      return [];
    }
    return pers.map((p, index: number) => ({
      nombre: p.nombre,
      cedula: p.cedula,
      direccion: p.direccion,
      telefono: p.telefono,
      avatarUrl: `/assets/images/avatar/avatar-${index + 1}.webp`, // o lógica para obtener la URL del avatar
    }));
  }

  const dataFiltered = applyFilter({
    inputData: mapDtoToProps(personas),
    comparator: getComparator(table.order, table.orderBy),
    filterName,
  });

  const notFound = !dataFiltered.length && !!filterName;

  return (
    <DashboardContent>
      <Box
        sx={{
          mb: 5,
          display: 'flex',
          alignItems: 'center',
        }}
      >
        <Typography variant="h4" sx={{ flexGrow: 1 }}>
          Personas
        </Typography>
        <Button
          variant="contained"
          color="inherit"
          startIcon={<Iconify icon="mingcute:add-line" />}
          onClick={() => setOpenDialog(true)}
        >
          Add Persona
        </Button>
      </Box>

      <PersonaFormDialog
        open={openDialog}
        onClose={() => setOpenDialog(false)}
        onCreated={handleCreated}
      />

      <Card>
        <PersonaTableToolbar
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
              <PersonaTableHead
                order={table.order}
                orderBy={table.orderBy}
                rowCount={_users.length}
                numSelected={table.selected.length}
                onSort={table.onSort}
                onSelectAllRows={(checked) =>
                  table.onSelectAllRows(
                    checked,
                    _users.map((user) => user.id)
                  )
                }
                headLabel={[
                  { id: 'nombre', label: 'Nombre' },
                  { id: 'cedula', label: 'Cédula' },
                  { id: 'telefono', label: 'Teléfono' },
                  { id: 'direccion', label: 'Dirección' },
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
                    <PersonaTableRow
                      key={row.cedula}
                      row={row}
                      selected={table.selected.includes(row.cedula)}
                      onSelectRow={() => table.onSelectRow(row.cedula)}
                    />
                  ))}

                <TableEmptyRows
                  height={68}
                  emptyRows={emptyRows(table.page, table.rowsPerPage, personas.length)}
                />

                {notFound && <TableNoData searchQuery={filterName} />}
              </TableBody>
            </Table>
          </TableContainer>
        </Scrollbar>

        <TablePagination
          component="div"
          page={table.page}
          count={personas.length}
          rowsPerPage={table.rowsPerPage}
          onPageChange={table.onChangePage}
          rowsPerPageOptions={[5, 10, 25]}
          onRowsPerPageChange={table.onChangeRowsPerPage}
        />
      </Card>
    </DashboardContent>
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
