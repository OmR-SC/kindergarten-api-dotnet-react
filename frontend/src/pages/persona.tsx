import { CONFIG } from 'src/config-global';

import { PersonaView } from 'src/sections/persona/view';

// ----------------------------------------------------------------------

export default function Page() {
  return (
    <>
      <title>{`Users - ${CONFIG.appName}`}</title>

      <PersonaView />
    </>
  );
}
