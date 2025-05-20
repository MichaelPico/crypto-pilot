export const msalConfig = {
    auth: {
        clientId: import.meta.env.VITE_AZURE_AD_CLIENT_ID || '',
        authority: `https://login.microsoftonline.com/${import.meta.env.VITE_MSAL_TENANT_ID}`,
    },
};

export const graphLoginRequest = {
    scopes: ['User.Read'],
};

export const apiLoginRequest = {
    scopes: [`api://${import.meta.env.VITE_AZURE_AD_CLIENT_ID}/user_impersonation`],
};

export const servicesConfig = {
    graphMeEndpoint: 'https://graph.microsoft.com/v1.0/me',
    graphAvatarEndpoint: 'https://graph.microsoft.com/v1.0/me/photos/48x48/$value',
    backendAPIEndpoint: import.meta.env.VITE_BACKEND_BASE_URL,
};

export const addServiceConfig = (service, url) => {
    servicesConfig[service] = url;
};