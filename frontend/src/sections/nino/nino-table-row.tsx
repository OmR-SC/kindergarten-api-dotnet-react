import { useState, useCallback } from 'react';

import Box from '@mui/material/Box';
import Avatar from '@mui/material/Avatar';
import Popover from '@mui/material/Popover';
import TableRow from '@mui/material/TableRow';
import Checkbox from '@mui/material/Checkbox';
import MenuList from '@mui/material/MenuList';
import TableCell from '@mui/material/TableCell';
import IconButton from '@mui/material/IconButton';
import MenuItem, { menuItemClasses } from '@mui/material/MenuItem';

//import { Label } from 'src/components/label';
import { Iconify } from 'src/components/iconify';

// ----------------------------------------------------------------------

export type NinoProps = {
  matricula: string;
  nombre: string;
  fechaNacimiento: string;
  fechaIngreso: string;
  fechaBaja: string | null;
  cedulaPagador: string;
  avatarUrl: string;
};

type NinoTableRowProps = {
  row: NinoProps;
  selected: boolean;
  onSelectRow: () => void;
  onEdit: (nino: NinoProps) => void;
  onDelete: (nino: NinoProps) => void;
};

export function NinoTableRow({ row, selected, onSelectRow, onEdit, onDelete }: NinoTableRowProps) {
  const [openPopover, setOpenPopover] = useState<HTMLButtonElement | null>(null);

  const handleOpenPopover = useCallback((event: React.MouseEvent<HTMLButtonElement>) => {
    setOpenPopover(event.currentTarget);
  }, []);

  const handleClosePopover = useCallback(() => {
    setOpenPopover(null);
  }, []);

  return (
    <>
      <TableRow hover tabIndex={-1} role="checkbox" selected={selected}>
        <TableCell padding="checkbox">
          <Checkbox disableRipple checked={selected} onChange={onSelectRow} />
        </TableCell>

        <TableCell component="th" scope="row">
          <Box
            sx={{
              gap: 2,
              display: 'flex',
              alignItems: 'center',
            }}
          >
            <Avatar alt={row.nombre} src={row.avatarUrl} />
            {row.nombre}
          </Box>
        </TableCell>

        <TableCell>{row.matricula}</TableCell>
        <TableCell>{row.fechaNacimiento}</TableCell>
        <TableCell>{row.fechaIngreso}</TableCell>
        <TableCell>{row.fechaBaja}</TableCell>
        <TableCell>{row.cedulaPagador}</TableCell>

        <TableCell align="right">
          <IconButton
            onClick={(e) => {
              e.stopPropagation();
              handleOpenPopover(e);
            }}
          >
            <Iconify icon="eva:more-vertical-fill" />
          </IconButton>
        </TableCell>
      </TableRow>

      <Popover
        open={!!openPopover}
        anchorEl={openPopover}
        onClose={handleClosePopover}
        anchorOrigin={{ vertical: 'top', horizontal: 'left' }}
        transformOrigin={{ vertical: 'top', horizontal: 'right' }}
      >
        <MenuList
          disablePadding
          sx={{
            p: 0.5,
            gap: 0.5,
            width: 140,
            display: 'flex',
            flexDirection: 'column',
            [`& .${menuItemClasses.root}`]: {
              px: 1,
              gap: 2,
              borderRadius: 0.75,
              [`&.${menuItemClasses.selected}`]: { bgcolor: 'action.selected' },
            },
          }}
        >
          <MenuItem
            onClick={() => {
              handleClosePopover();
              onEdit(row); // <— pasar row al handler de edición
            }}
          >
            <Iconify icon="solar:pen-bold" />
            Edit
          </MenuItem>

          <MenuItem
            onClick={() => {
              handleClosePopover();
              onDelete(row);
            }}
            sx={{ color: 'error.main' }}
          >
            <Iconify icon="solar:trash-bin-trash-bold" />
            Delete
          </MenuItem>
        </MenuList>
      </Popover>
    </>
  );
}
