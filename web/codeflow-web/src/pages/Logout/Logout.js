import { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../features/Auth/AuthProvider "; 

function Logout() {
  const navigate = useNavigate();
  const { logout } = useAuth();

  useEffect(() => {
    (async () => {
      await logout();
          navigate("/login", { replace: true });
    })();
  }, [logout, navigate]);

  return <div>Logging out…</div>;
}

export default Logout;
