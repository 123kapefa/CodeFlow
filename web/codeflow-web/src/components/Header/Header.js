import React from "react";
import { Link } from "react-router-dom";
import { useAuth } from '../../features/Auth/AuthProvider ';

import "../../App.css";

function Header() {
  const { user, logout, loading } = useAuth();   // ← теперь есть user.avatarUrl  

  return (
    <div className="container-xxl">
      <header className="topbar p-2">
        <div className="logo">
          <Link to="/home">
            <img src="/logo/logo-transparent.png" alt="logo" />
          </Link>
        </div>

        <form method="get" className="search">
          <input name="q" placeholder="Search…" />
        </form>

        {/* пока грузимся — ничего не показываем */}
        {loading ? null : user ? (
          /* --------- Авторизован --------- */
          <ul className="topbar__icons list-unstyled d-flex m-0">
            <li>
              <Link
                to={`/users/${user.userId}`}   // профиль текущего юзера
                className="me-2"
              >
                <img
                  src={
                    user.avatarUrl?.trim()
                      ? user.avatarUrl
                      : '/avatar/avatar_default.png'
                  }
                  alt="avatar"
                  width={32}
                  height={32}
                  style={{ borderRadius: 0, objectFit: 'cover' }}
                />
              </Link>
            </li>
            <li>
              <button className="btn btn-primary" onClick={logout}>
                Logout
              </button>
            </li>
          </ul>
        ) : (
          /* --------- Не авторизован --------- */
          <div>
            <Link to="/login"  className="btn btn-outline-primary me-2">Log in</Link>
            <Link to="/signup" className="btn btn-primary">Sign up</Link>
          </div>
        )}
      </header>
    </div>
  );
}

export default Header;