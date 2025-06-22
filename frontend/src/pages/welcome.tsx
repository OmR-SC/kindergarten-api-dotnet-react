import { CONFIG } from 'src/config-global';

import { WelcomeView } from 'src/sections/welcome/view';

// ----------------------------------------------------------------------

export default function Page() {
  return (
    <>
      <title>{`Welcome
       - ${CONFIG.appName}`}</title>

      <WelcomeView />
    </>
  );
}
