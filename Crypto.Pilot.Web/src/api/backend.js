import { PublicClientApplication, InteractionRequiredAuthError } from '@azure/msal-browser';
import { msalConfig, graphLoginRequest } from '../AuthConfig';

const msalInstance = new PublicClientApplication(msalConfig);
let msalInitialized = false;

async function initializeMsal() {
  if (!msalInitialized) {
    await msalInstance.initialize();
    msalInitialized = true;
  }
}

export async function apiFetch(path, options = {}) {
  await initializeMsal();

  const BASE_URL = import.meta.env.VITE_BACKEND_BASE_URL;
  const url = `${BASE_URL}${path}`;

  const {
    method = 'GET',
    queryParams = [],
    body = null,
    headers = {},
    requiredRoles = [],
  } = options;

  const accounts = msalInstance.getAllAccounts();
  const currentAccount = accounts[0];

  if (!currentAccount) {
    throw new Error('No accounts detected');
  }

  const response = await msalInstance.acquireTokenSilent({
    ...graphLoginRequest,
    account: currentAccount,
  });

  if (requiredRoles.length > 0) {
    if (
      !response?.account ||
      !response.account?.idTokenClaims ||
      !requiredRoles.some((role) => response.account?.idTokenClaims.roles?.includes(role))
    ) {
      throw new Error('User does not have required role');
    }
  }

  const headersWithAuth = {
    ...headers,
    Authorization: `Bearer ${response.accessToken}`,
    'Content-Type': 'application/json',
  };

  const searchParams = new URLSearchParams();
  queryParams.forEach((param) => {
    searchParams.append(param.key, param.value);
  });
  const queryString = searchParams.toString();
  const urlWithParams = queryString ? `${url}?${queryString}` : url;

  try {
    const fetchResponse = await fetch(urlWithParams, { method, headers: headersWithAuth, body });
    if (!fetchResponse.ok) {
      throw new Error(`API error: ${fetchResponse.status}`);
    }
    return fetchResponse.json();
  } catch (err) {
    if (err instanceof InteractionRequiredAuthError || err.errorCode === 'monitor_window_timeout') {
      msalInstance.acquireTokenRedirect({
        ...graphLoginRequest,
        account: currentAccount,
      });
    }
     throw err;
  }
}
