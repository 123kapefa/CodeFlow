import React, { useState } from "react";
import { Link } from "react-router-dom";
import { useAuth } from "../../features/Auth/AuthProvider ";

import "./Header.css";

function Header({ onBurgerClick }) {
  const { user, logout, loading } = useAuth();
  const [mobileSearchOpen, setMobileSearchOpen] = useState(false);

  return (
    <div className="container-xxl">
      <header className="topbar p-2">
        {/* Бургер для мобильных */}
        <button
          className="btn btn-outline-secondary d-lg-none"
          aria-label="Open menu"
          onClick={onBurgerClick}
        >
          <i className="bi bi-list" aria-hidden="true"></i>
        </button>

        <div className="logo">
          <Link to="/home">
            <img src="/logo/logo-transparent.png" alt="logo" />
          </Link>
        </div>

        {/* Десктопный поиск (только >= lg) */}
        <form method="get" className="search d-none d-lg-block">
          <input name="q" placeholder="Search…" />
        </form>

        {/* Правая часть */}
        {loading ? null : user ? (
          <div className="ms-auto d-flex align-items-center gap-2">
            {" "}
            {/* ← добавили ms-auto */}
            {/* Кнопка лупы — только на мобильных */}
            <button
              type="button"
              className="btn btn-outline-secondary d-lg-none"
              aria-label="Search"
              onClick={() => setMobileSearchOpen((v) => !v)}
            >
              <i className="bi bi-search" aria-hidden="true"></i>
            </button>
            <ul className="topbar__icons list-unstyled d-flex m-0">
              <li>
                <Link to={`/users/${user.userId}`} className="me-2">
                  <img
                    src={
                      user.avatarUrl?.trim()
                        ? user.avatarUrl
                        : "/avatar/avatar_default.png"
                    }
                    alt="avatar"
                    width={32}
                    height={32}
                    style={{ borderRadius: 0, objectFit: "cover" }}
                  />
                </Link>
              </li>
              <li>
                <button className="btn btn-primary" onClick={logout}>
                  Logout
                </button>
              </li>
            </ul>
          </div>
        ) : (
          <div className="ms-auto d-flex align-items-center gap-2">
            {" "}
            {/* ← добавили ms-auto */}
            {/* Кнопка лупы — только на мобильных */}
            <button
              type="button"
              className="btn btn-outline-secondary d-lg-none"
              aria-label="Search"
              onClick={() => setMobileSearchOpen((v) => !v)}
            >
              <i className="bi bi-search" aria-hidden="true"></i>
            </button>
            <Link to="/login" className="btn btn-outline-primary me-2">
              Log in
            </Link>
            <Link to="/signup" className="btn btn-primary">
              Sign up
            </Link>
          </div>
        )}
      </header>

      {/* Мобильное выпадающее поле поиска (только < lg) */}
      {!loading && mobileSearchOpen && (
        <div className="mobile-search d-lg-none px-2 pb-2">
          <form method="get" className="w-100">
            <input name="q" placeholder="Search…" />
          </form>
        </div>
      )}
    </div>
  );
}

export default Header;
