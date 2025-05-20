import React from 'react';
import { createRoot } from 'react-dom/client';
import App from './App.jsx';
import { PublicClientApplication, EventType } from '@azure/msal-browser';
import { msalConfig } from './AuthConfig.js';

const msalInstance = new PublicClientApplication(msalConfig);

// Default to using the first account if no account is active on page load
if (!msalInstance.getActiveAccount() && msalInstance.getAllAccounts().length > 0) {
    msalInstance.setActiveAccount(msalInstance.getAllAccounts()[0]);
}

// Listen for sign-in event and set active account
msalInstance.addEventCallback((event) => {
    if (event.eventType === EventType.LOGIN_SUCCESS && event.payload.account) {
        const account = event.payload.account;
        msalInstance.setActiveAccount(account);
    }
});

const root = createRoot(document.getElementById('root'));
root.render(
    <App instance={msalInstance} />
);
