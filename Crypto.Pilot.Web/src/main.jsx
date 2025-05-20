import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { MsalProvider, useMsal } from '@azure/msal-react';
import { PublicClientApplication } from '@azure/msal-browser';
import App from './App.jsx';

const msalConfig = {
  auth: {
    clientId: import.meta.env.VITE_AZURE_AD_CLIENT_ID,
    authority: import.meta.env.VITE_AZURE_AD_AUTHORITY,
    redirectUri: window.location.origin,
  },
};

const msalInstance = new PublicClientApplication(msalConfig);

function AppWithLogin() {
  const { instance, accounts, inProgress } = useMsal();

  if (accounts.length > 0) {
    return <span>There are currently {accounts.length} users signed in!</span>;
  } else if (inProgress === 'login') {
    return <span>Login is currently in progress!</span>;
  } else {
    return (
      <>
        <span>There are currently no users signed in!</span>
        <button
          onClick={() =>
            instance.loginPopup({
              scopes: [import.meta.env.VITE_AZURE_FUNCTION_API_SCOPE],
            })
          }
        >
          Login
        </button>
      </>
    );
  }
}

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <MsalProvider instance={msalInstance}>
      <AppWithLogin />
    </MsalProvider>
  </StrictMode>
);
