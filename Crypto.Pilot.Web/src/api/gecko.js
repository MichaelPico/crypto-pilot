import { apiFetch } from './backend';

export function getCoins() {
  return apiFetch('GetCoins');
}

export function getCurrencies() {
  return apiFetch('GetCurrencies');
}

export function getCoinsPrice({
  vs_currencies = 'eur',
  ids = 'bitcoin,solana',
  include_market_cap = false,
  include_24hr_vol = false,
  include_24hr_change = false,
  include_last_updated_at = false,
  precision = 2,
} = {}) {
  const params = new URLSearchParams({
    vs_currencies,
    ids,
    include_market_cap,
    include_24hr_vol,
    include_24hr_change,
    include_last_updated_at,
    precision,
  });
  return apiFetch('GetCoinsPrice?' + params.toString());
}
