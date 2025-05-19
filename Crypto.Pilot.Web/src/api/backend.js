const BASE_URL = import.meta.env.VITE_BACKEND_BASE_URL;
const API_KEY = import.meta.env.VITE_BACKEND_KEY;

export async function apiFetch(path, options = {}) {
  const url = BASE_URL + path;
  const headers = {
    ...(options.headers || {}),
    'x-functions-key': API_KEY,
    'Content-Type': 'application/json',
  };
  const response = await fetch(url, { ...options, headers });
  if (!response.ok) {
    throw new Error(`API error: ${response.status}`);
  }
  return response.json();
}
