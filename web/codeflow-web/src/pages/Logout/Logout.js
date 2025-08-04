import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import Cookies from "js-cookie";

function Logout() {

  const navigate = useNavigate();
  const [error, setError] = useState('');

  useEffect(() => {
    const logout = async () => {
      try {
        const refreshToken = Cookies.get("refresh_token");

        if (!refreshToken) {          
          navigate("/login", { replace: true });
          return;
        }

        
        const response = await fetch("http://localhost:5000/api/auth/logout", {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(refreshToken) 
        });

        if (!response.ok) {
          const err = await response.json();
          throw new Error(err.message || "Ошибка выхода");
        }

        // Чистим куки
        Cookies.remove("jwt");
        Cookies.remove("refresh_token");

        // Редиректим на login
        navigate("/login", { replace: true });
      }
       catch (err) {
        setError(err.message);
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