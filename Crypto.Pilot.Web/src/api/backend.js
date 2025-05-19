const BASE_URL = import.meta.env.VITE_BACKEND_BASE_URL;
const API_KEY = import.meta.env.VITE_BACKEND_KEY;

export async function apiFetch(path, options = {}) {
  let url = BASE_URL + path;
  // Add the API key as a 'code' query parameter
  const urlObj = new URL(url, window.location.origin);
  urlObj.searchParams.set('code', API_KEY);
  url = urlObj.toString();

  const headers = {
    ...(options.headers || {}),
    'Content-Type': 'application/json',
  };
  const response = await fetch(url, { ...options, headers });
  if (!response.ok) {
    throw new Error(`API error: ${response.status}`);
  }
  return response.json();
}
