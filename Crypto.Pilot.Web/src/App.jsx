import React, { useState } from "react";
import { MsalProvider, MsalAuthenticationTemplate } from "@azure/msal-react";
import { PublicClientApplication, InteractionType } from "@azure/msal-browser";
import { msalConfig, graphLoginRequest } from "./AuthConfig";
import { CssBaseline, ThemeProvider } from "@mui/material";
import BasicLayout from "./components/BasicLayout";
import { lightTheme, darkTheme } from "./theme/theme";

function App() {
  const msalInstance = new PublicClientApplication(msalConfig);
  const [isDarkMode, setIsDarkMode] = useState(false);

  const toggleTheme = () => {
    setIsDarkMode(!isDarkMode);
  };

  return (
    <MsalProvider instance={msalInstance}>
      <ThemeProvider theme={isDarkMode ? darkTheme : lightTheme}>
        <CssBaseline />
        <MsalAuthenticationTemplate
          interactionType={InteractionType.Redirect}
          authenticationRequest={graphLoginRequest}
        >
          <BasicLayout toggleTheme={toggleTheme} />
        </MsalAuthenticationTemplate>
      </ThemeProvider>
    </MsalProvider>
  );
}

export default App;
