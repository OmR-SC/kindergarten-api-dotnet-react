import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';

import { CONFIG } from 'src/config-global';

import { NinoView } from 'src/sections/nino/view';


// ----------------------------------------------------------------------

export default function Page() {
  return (
    <>
      <title>{`Users - ${CONFIG.appName}`}</title>

      <LocalizationProvider dateAdapter={AdapterDayjs}>
        <NinoView />
      </LocalizationProvider>
    </>
  );
}
