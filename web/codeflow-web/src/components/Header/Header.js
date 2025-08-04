import React from "react";
import { Link } from "react-router-dom";
import Cookies from "js-cookie";

import "../../App.css";

function Header({ isAuthenticated }) {
  return (
    <div className="container-xxl">
      <header className="topbar p-2">
        <div className="logo ">
          <Link to="/">
            <img src="/logo/logo-transparent.png" alt="logo" />
          </Link>
        </div>

        <form method="get" className="search">
          <input name="q" placeholder="Search…" />
        </form>

        {/* Если пользователь авторизован  */}
        {isAuthenticated ? (
          <ul className="topbar__icons list-unstyled d-flex m-0">
           
            <li>
              <Link to="/user_page" className="me-2">  {/* TODO заменить на адрес стр. юзера */}
                
                <img
                  src="/avatar/avatar_default.png"
                  alt="avatar"
                  width={32}
                  height={32}
                  style={{ borderRadius: "50%", objectFit: "cover" }}
                />
              </Link>
            </li>
            <li>
              <Link to="/logout" className="btn btn-primary">Logout</Link>
            </li>
          </ul>
        ) : (
          // Если НЕ авторизован
          <div>
            <Link to="/login" className="btn btn-outline-primary me-2">
              Log in
            </Link>
            <Link to="/signup" className="btn btn-primary">
              Sign up
            </Link>
          </div>
        )}
      </header>
    </div>
  );
}

export default Header;
