import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import Cookies from "js-cookie";
import { RefreshToken } from "../authClient";

function Logout() {
  const navigate = useNavigate();
  const [error, setError] = useState("");

  useEffect(() => {
    const logout = async () => {
      try {
        const refreshToken = Cookies.get("refresh_token");
        const token = Cookies.get("jwt");

        if (!refreshToken) {
          navigate("/login", { replace: true });
          return;
        }

        const headers = { "Content-Type": "application/json" };
        if (token) headers.Authorization = `Bearer ${token}`;

        const response = await fetch("http://localhost:5000/api/auth/logout", {
          method: "POST",
          headers,
          body: JSON.stringify( refreshToken ),
        });

        if (response.status === 401) {
          const refresh_response = RefreshToken();

          if (refresh_response) {
            response = await fetch("http://localhost:5000/api/auth/logout", {
              method: "POST",
              headers,
              body: JSON.stringify( refreshToken ),
            });
          }
        }

        if (!response.ok) {
          const err = await response.json();   
          throw new Error(err.message || "Ошибка выхода");
        }
      } 
      catch (err) {
        setError(err.message);
      } 
      finally {
        Cookies.remove("jwt", { path: "/" });
        Cookies.remove("refresh_token", { path: "/" });
        navigate("/login", { replace: true });
      }
    };

    logout();
  }, [navigate]);

  return (
    <div className="d-flex justify-content-center align-items-center vh-100">
      {error ? (
        <div className="alert alert-danger">{error}</div>
      ) : (
        <div>Logging out...</div>
      )}
    </div>
  );
}

export default Logout;
