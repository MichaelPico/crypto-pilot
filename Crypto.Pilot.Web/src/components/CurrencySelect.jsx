import { useEffect, useState } from 'react';
import { getCurrencies } from '../api/gecko';
// Material UI imports
import Autocomplete from '@mui/material/Autocomplete';
import TextField from '@mui/material/TextField';
import Box from '@mui/material/Box';

let cachedCurrencies = null;

function CurrencySelect() {
  const [currencies, setCurrencies] = useState([]);
  const [selected, setSelected] = useState('');

  useEffect(() => {
    if (cachedCurrencies) {
      setCurrencies(cachedCurrencies);
    } else {
      getCurrencies()
        .then(list => {
          cachedCurrencies = list;
          setCurrencies(list);
        })
        .catch((error) => {
          console.error('Failed to fetch currencies:', error);
          setCurrencies([]);
        });
    }
  }, []);

  return (
    <Box sx={{ minWidth: 200 }}>
      <Autocomplete
        options={currencies}
        value={selected}
        onChange={(_, value) => setSelected(value || '')}
        renderInput={(params) => (
          <TextField {...params} label="Select currency" variant="outlined" size="small" />
        )}
        getOptionLabel={option => option || ''}
        isOptionEqualToValue={(option, value) => option === value}
        clearOnEscape
      />
      {selected && <div style={{ marginTop: 8 }}>Selected: {selected}</div>}
    </Box>
  );
}

export default CurrencySelect;
