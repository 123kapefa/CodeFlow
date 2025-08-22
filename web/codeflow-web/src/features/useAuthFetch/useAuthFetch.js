import Cookies from "js-cookie";
import { RefreshToken } from "../RefreshToken/RefreshToken";
import { useCallback } from "react";


export const useAuthFetch = () => {
  // useCallback гарантирует одну и ту же ссылку на функцию между рендерами
  const authFetch = useCallback(async (url, options = {}) => {

    let at = Cookies.get("jwt");

    const baseOpts = {
      ...options,
      headers: {
        ...(options.headers ?? {}),
        "Content-Type": options.body
          ? "application/json"
          : options.headers?.["Content-Type"] ?? undefined,
        Accept: "application/json",
        ...(at ? { Authorization: `Bearer ${at}` } : {}),
      },
      credentials: "include", // тянем refresh-cookie

    };

    let res = await fetch(url, baseOpts);

    if (res.status === 401) {
      const ok = await RefreshToken();
      if (ok) {
        at = Cookies.get("jwt");
        res = await fetch(url, {
          ...baseOpts,
          headers: { ...baseOpts.headers, Authorization: `Bearer ${at}` },
        });
      }
    }
    return res;
  }, []);

  return authFetch;
};
