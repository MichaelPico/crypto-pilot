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
  const response = await fetch(`${import.meta.env.VITE_BACKEND_BASE_URL}/.auth/me`);
  if (!response.ok) {
    throw new Error('Failed to fetch managed identity token');
  }
  const data = await response.json();
  const token = data[0]?.access_token;
  if (!token) {
    throw new Error('No access token found in managed identity response');
  }
  return token;
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
