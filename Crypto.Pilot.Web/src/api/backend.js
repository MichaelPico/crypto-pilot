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

let msalInitialized = false;

async function initializeMsal() {
  if (!msalInitialized) {
    try {
      await msalInstance.initialize();
      msalInitialized = true;
    } catch (error) {
      console.error('Failed to initialize MSAL:', error);
      throw error;
    }
  }
}

async function getAccessToken() {
  await initializeMsal(); // Ensure MSAL is initialized before proceeding

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
