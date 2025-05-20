import React from 'react';
import {
  AppBar,
  Box,
  Container,
  Button,
  Toolbar,
  Typography,
  useTheme,
} from '@mui/material';
import UserProfile from './UserProfile';
import { CurrencyInfo } from './CurrencyInfo';
import { AlertCreator } from './AlertCreator';

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
          <Button
            sx={{ mr: 2 }}
            onClick={toggleTheme}
            color="inherit"
            variant="outlined"
            size="small"
          >
            {theme.palette.mode === 'dark' ? 'Light Mode' : 'Dark Mode'}
          </Button>
          <UserProfile />
        </Toolbar>
      </AppBar>
      <Container component="main" sx={{ mt: 4, mb: 4, flexGrow: 1 }}>
        <CurrencyInfo />
        <AlertCreator />
      </Container>
    </Box>
  );
}

export default BasicLayout;
