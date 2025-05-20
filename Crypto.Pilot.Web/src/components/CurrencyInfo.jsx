import React, { useState, useEffect } from 'react';
import { getCoins, getCurrencies, getCoinsPrice } from '../api/gecko';
import { 
    Autocomplete, 
    TextField, 
    FormControl, 
    Paper, 
    Typography,
    Box,
    Button,
    CircularProgress,
    Container
} from '@mui/material';

export function CurrencyInfo() {
    const [coins, setCoins] = useState([]);
    const [currencies, setCurrencies] = useState([]);
    const [selectedCoin, setSelectedCoin] = useState(null);
    const [selectedCurrency, setSelectedCurrency] = useState('eur');
    const [priceData, setPriceData] = useState(null);
    const [isLoading, setIsLoading] = useState(true);
    const [isPriceLoading, setIsPriceLoading] = useState(false);

    useEffect(() => {
        const loadData = async () => {
            setIsLoading(true);
            try {
                const [coinsData, currenciesData] = await Promise.all([
                    getCoins(),
                    getCurrencies()
                ]);

                // Debugging: Log the data to ensure it's correct
                console.log('Coins Data:', coinsData);
                console.log('Currencies Data:', currenciesData);

                // Filter coins to include only the ten most popular by id
                const popularCoinIds = [
                    'bitcoin', 'ethereum', 'litecoin', 'dogecoin', 
                    'solana', 'ripple', 'cardano', 'polkadot', 
                    'binancecoin', 'shiba-inu'
                ];
                const filteredCoins = coinsData.filter(coin => popularCoinIds.includes(coin.id));
                setCoins(filteredCoins);

                // Transform currencies array into objects with label and value
                const transformedCurrencies = currenciesData.map(currency => ({
                    label: currency.toUpperCase(),
                    value: currency
                }));
                setCurrencies(transformedCurrencies);
            } catch (error) {
                console.error('Failed to load initial data:', error);
            } finally {
                setIsLoading(false);
            }
        };
        loadData();
    }, []);

    const handleGetPrices = async () => {
        if (!selectedCoin || !selectedCurrency) return;

        setIsPriceLoading(true);
        try {
            const prices = await getCoinsPrice({
                vs_currencies: selectedCurrency,
                ids: selectedCoin.id,
                include_market_cap: true,
                include_24hr_vol: true,
                include_24hr_change: true,
                include_last_updated_at: true
            });
            setPriceData(prices);
        } catch (error) {
            console.error('Failed to fetch prices:', error);
        } finally {
            setIsPriceLoading(false);
        }
    };

    if (isLoading) {
        return (
            <Box sx={{ display: 'flex', justifyContent: 'center', p: 3 }}>
                <CircularProgress />
            </Box>
        );
    }

    return (
        <Container maxWidth={false} sx={{ p: 3 }}>
            <Typography variant="h4" gutterBottom>
                Coin Info
            </Typography>
            <Box sx={{ display: 'flex', flexDirection: 'column', gap: 3, p: 3 }}>
                <Paper sx={{ p: 2 }}>
                    <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
                        <Autocomplete
                            options={coins}
                            getOptionLabel={(option) => option.name || 'Unknown'}
                            value={selectedCoin}
                            onChange={(event, newValue) => setSelectedCoin(newValue)}
                            renderInput={(params) => (
                                <TextField {...params} label="Select Coin" variant="outlined" fullWidth />
                            )}
                            isOptionEqualToValue={(option, value) => option.id === value.id}
                        />
                        <FormControl fullWidth>
                            <TextField
                                select
                                label="Currency"
                                value={selectedCurrency}
                                onChange={(e) => setSelectedCurrency(e.target.value)}
                                SelectProps={{
                                    native: true,
                                }}
                            >
                                {currencies.map((currency) => (
                                    <option key={currency.value} value={currency.value}>
                                        {currency.label}
                                    </option>
                                ))}
                            </TextField>
                        </FormControl>
                        <Button 
                            variant="contained" 
                            onClick={handleGetPrices}
                            disabled={!selectedCoin || !selectedCurrency || isPriceLoading}
                            fullWidth
                        >
                            {isPriceLoading ? <CircularProgress size={24} color="inherit" /> : 'Get Prices'}
                        </Button>
                    </Box>
                </Paper>
                {priceData && (
                    <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
                        {Object.entries(priceData).map(([coinId, data]) => (
                            <Paper key={coinId} sx={{ p: 2 }}>
                                <Typography variant="h6" gutterBottom>
                                    {coins.find((c) => c.id === coinId)?.name || coinId}
                                </Typography>
                                <Typography variant="body1">
                                    Price: {data[selectedCurrency]} {selectedCurrency.toUpperCase()}
                                </Typography>
                                <Typography variant="body2" color="text.secondary">
                                    Market Cap: {data[selectedCurrency + '_market_cap']}
                                </Typography>
                                <Typography variant="body2" color="text.secondary">
                                    24h Volume: {data[selectedCurrency + '_24h_vol']}
                                </Typography>
                                <Typography 
                                    variant="body2" 
                                    color={data[selectedCurrency + '_24h_change'] > 0 ? 'success.main' : 'error.main'}
                                >
                                    24h Change: {data[selectedCurrency + '_24h_change']}%
                                </Typography>
                                <Typography variant="caption" display="block" color="text.secondary">
                                    Last Updated: {new Date(data.last_updated_at * 1000).toLocaleString()}
                                </Typography>
                            </Paper>
                        ))}
                    </Box>
                )}
            </Box>
        </Container>
    );
}
