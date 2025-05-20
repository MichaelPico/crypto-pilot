import React from 'react';
import {
  AppBar,
  Box,
  Container,
  IconButton,
  Toolbar,
  Typography,
  useTheme,
} from '@mui/material';
import { Brightness4, Brightness7 } from '@mui/icons-material';
import CurrencySelect from './CurrencySelect';
import UserProfile from './UserProfile';

function BasicLayout({ toggleTheme }) {
  const theme = useTheme();

  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', minHeight: '100vh' }}>
      <AppBar position="static">
        <Toolbar>
          <Typography
            variant="h6"
            component="div"
            sx={{
              flexGrow: 1,
              display: 'flex',
              alignItems: 'center',
              gap: 2,
            }}
          >
            <img
              src="/crypto-pilot-logo.svg"
              alt="Crypto Pilot"
              style={{ height: '32px' }}
            />
            Crypto Pilot
          </Typography>
          <IconButton
            sx={{ mr: 2 }}
            onClick={toggleTheme}
            color="inherit"
          >
            {theme.palette.mode === 'dark' ? <Brightness7 /> : <Brightness4 />}
          </IconButton>
          <UserProfile />
        </Toolbar>
      </AppBar>
      <Container component="main" sx={{ mt: 4, mb: 4, flexGrow: 1 }}>
        <CurrencySelect />
      </Container>
    </Box>
  );
}

export default BasicLayout;
