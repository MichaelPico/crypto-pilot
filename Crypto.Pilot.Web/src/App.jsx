import React from "react";
import { MsalProvider, MsalAuthenticationTemplate } from "@azure/msal-react";
import { PublicClientApplication, InteractionType } from "@azure/msal-browser";
import { msalConfig, graphLoginRequest } from "./AuthConfig";
import { CssBaseline, ThemeProvider } from "@mui/material";
import BasicLayout from "./components/BasicLayout";

function App() {
  const msalInstance = new PublicClientApplication(msalConfig);

  return (
    <MsalProvider instance={msalInstance}>
      <MsalAuthenticationTemplate
        interactionType={InteractionType.Redirect}
        authenticationRequest={graphLoginRequest}
      >
        <CssBaseline />
        <BasicLayout />
      </MsalAuthenticationTemplate>
    </MsalProvider>
  );
}

export default App;
