import { createTheme } from '@mui/material';

export const lightTheme = createTheme({
  palette: {
    mode: 'light',
    primary: {
      main: '#646cff',
    },
    background: {
      default: '#ffffff',
      paper: '#f5f5f5',
    },
  },
});

export const darkTheme = createTheme({
  palette: {
    mode: 'dark',
    primary: {
      main: '#747bff',
    },
    background: {
      default: '#242424',
      paper: '#1a1a1a',
    },
  },
});
