import { apiFetch } from './backend';

// USERS
export function getAllUsers() {
  return apiFetch('database/users');
}

export function upsertUser({ id = -1, name, email, phoneNumber }) {
  return apiFetch('database/users', {
    method: 'POST',
    body: JSON.stringify({
      id,
      name,
      email,
      phoneNumber,
    }),
  });
}

// CRYPTOCURRENCIES
export function getAllCryptocurrencies() {
  return apiFetch('database/cryptocurrencies');
}

export function upsertCryptocurrency({ id = -1, name, currentPrice }) {
  return apiFetch('database/cryptocurrencies', {
    method: 'POST',
    body: JSON.stringify({
      id,
      name,
      currentPrice,
    }),
  });
}

// ALERTS
export function getAllAlerts() {
  return apiFetch('database/alerts');
}

export function upsertAlert({ id = -1, userId, cryptocurrencyId, targetPrice, notified, overThePrice }) {
  return apiFetch('database/alerts', {
    method: 'POST',
    body: JSON.stringify({
      id,
      userId,
      cryptocurrencyId,
      targetPrice,
      notified,
      overThePrice,
    }),
  });
}
