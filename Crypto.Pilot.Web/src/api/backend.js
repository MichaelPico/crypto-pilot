import { PublicClientApplication } from '@azure/msal-browser';

const msalConfig = {
  auth: {
    clientId: import.meta.env.VITE_AZURE_AD_CLIENT_ID,
    authority: import.meta.env.VITE_AZURE_AD_AUTHORITY, // e.g. https://login.microsoftonline.com/<tenant-id>
    redirectUri: window.location.origin,
  }
};
const apiScope = import.meta.env.VITE_AZURE_FUNCTION_API_SCOPE; // e.g. api://<function-app-client-id>/user_impersonation

const msalInstance = new PublicClientApplication(msalConfig);

// Ensure MSAL is initialized before using it
await msalInstance.initialize();

async function getAccessToken() {
  const accounts = msalInstance.getAllAccounts();
  if (accounts.length === 0) {
    await msalInstance.loginPopup({ scopes: [apiScope] });
  }
  const account = msalInstance.getAllAccounts()[0];
  const result = await msalInstance.acquireTokenSilent({
    account,
    scopes: [apiScope],
  });
  return result.accessToken;
}

export async function apiFetch(path, options = {}) {
  const BASE_URL = import.meta.env.VITE_BACKEND_BASE_URL;
  let url = BASE_URL + path;

  const token = await getAccessToken();

  const headers = {
    ...(options.headers || {}),
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${token}`,
  };
  const response = await fetch(url, { ...options, headers });
  if (!response.ok) {
    throw new Error(`API error: ${response.status}`);
  }
  return response.json();
}
