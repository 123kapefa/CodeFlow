import { API_BASE } from "../../config";

export function redirectToProvider(provider, returnUrl = "/") {
  const url =
    `${API_BASE}/auth/external/challenge/${provider}` +
    `?returnUrl=${encodeURIComponent(returnUrl)}`;
  window.location.assign(url);
}