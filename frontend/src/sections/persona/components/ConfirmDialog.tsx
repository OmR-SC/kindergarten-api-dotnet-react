import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Typography,
} from '@mui/material';

type ConfirmDialogProps = {
  open: boolean;
  title?: string;
  content: string;
  onClose: () => void;
  onConfirm: () => void;
};

export function ConfirmDialog({
  open,
  title = 'Confirm',
  content,
  onClose,
  onConfirm,
}: ConfirmDialogProps) {
  return (
    <Dialog open={open} onClose={onClose} maxWidth="xs" fullWidth>
      <DialogTitle>{title}</DialogTitle>
      <DialogContent>
        <Typography>{content}</Typography>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Cancel</Button>
        <Button color="error" onClick={onConfirm} variant="contained">
          Delete
        </Button>
      </DialogActions>
    </Dialog>
  );
}
