import React, { useState, useEffect } from 'react';
import { getCoins } from '../api/gecko';
import { upsertUser, upsertAlert } from '../api/database';
import {
    Autocomplete,
    TextField,
    FormControl,
    Paper,
    Typography,
    Box,
    Button,
    CircularProgress,
    Container,
    Switch,
    FormControlLabel,
} from '@mui/material';
import { useMsal } from '@azure/msal-react';

export function AlertCreator() {
    const { accounts } = useMsal();
    const [coins, setCoins] = useState([]);
    const [selectedCoin, setSelectedCoin] = useState(null);
    const [targetPrice, setTargetPrice] = useState('');
    const [over, setOver] = useState(true);
    const [isLoading, setIsLoading] = useState(true);
    const [isSubmitting, setIsSubmitting] = useState(false);

    const user = accounts[0] || {}; // Get the first account from MSAL
    const userName = user.name || 'Unknown User';
    const userEmail = user.username || 'unknown@example.com';

    useEffect(() => {
        const loadCoins = async () => {
            setIsLoading(true);
            try {
                const coinsData = await getCoins();

                // Filter coins to include only the ten most popular by id
                const popularCoinIds = [
                    'bitcoin', 'ethereum', 'litecoin', 'dogecoin',
                    'solana', 'ripple', 'cardano', 'polkadot',
                    'binancecoin', 'shiba-inu'
                ];
                const filteredCoins = coinsData.filter(coin => popularCoinIds.includes(coin.id));
                setCoins(filteredCoins);
            } catch (error) {
                console.error('Failed to load coins:', error);
            } finally {
                setIsLoading(false);
            }
        };
        loadCoins();
    }, []);

    const handleSubmit = async () => {
        if (!selectedCoin || !targetPrice) {
            console.error('Submission failed: Missing required fields.');
            return;
        }

        setIsSubmitting(true);
        try {
            console.log('Submitting alert...');
            console.log('User Info:', { userName, userEmail });
            console.log('Selected Coin:', selectedCoin);
            console.log('Target Price:', targetPrice);
            console.log('Over:', over);

            // Upsert user
            const userPayload = {
                id: 1, // Example user ID
                name: userName,
                email: userEmail,
                phoneNumber: '1234567890', // Placeholder phone number
            };
            console.log('Upserting user:', userPayload);
            await upsertUser(userPayload);

            // Upsert alert
            const alertPayload = {
                id: -1, // New alert
                userId: userPayload.id,
                cryptocurrencyId: selectedCoin.id,
                targetPrice: parseFloat(targetPrice),
                notified: false,
                overThePrice: over,
            };
            console.log('Upserting alert:', alertPayload);
            await upsertAlert(alertPayload);

            console.log('Alert created successfully!');
            alert('Alert created successfully!');
        } catch (error) {
            console.error('Failed to create alert:', error);
            alert('Failed to create alert. Please try again.');
        } finally {
            setIsSubmitting(false);
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
                Create Alert
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
                        <TextField
                            label="Target Price"
                            type="number"
                            value={targetPrice}
                            onChange={(e) => setTargetPrice(e.target.value)}
                            fullWidth
                        />
                        <FormControlLabel
                            control={
                                <Switch
                                    checked={over}
                                    onChange={(e) => setOver(e.target.checked)}
                                />
                            }
                            label={over ? 'Over the Price' : 'Under the Price'}
                        />
                        <Button
                            variant="contained"
                            onClick={handleSubmit}
                            disabled={!selectedCoin || !targetPrice || isSubmitting}
                            fullWidth
                        >
                            {isSubmitting ? <CircularProgress size={24} color="inherit" /> : 'Create Alert'}
                        </Button>
                    </Box>
                </Paper>
            </Box>
        </Container>
    );
}
