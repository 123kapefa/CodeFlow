import React from "react";
import { Link } from 'react-router-dom';

import '../../App.css'

function Header() {
  return (
  <div className="container-xxl">     
       <header className="topbar">
        <div className="logo">
            <Link to="/">
                <img src="/logo/logo-transparent.png" alt="logo" />                
            </Link>
        </div>

        <form method="get" className="search">
            <input name="q" placeholder="Search…" />
        </form>

        <ul className="topbar__icons">
            <li><Link to="/questions"><i className="bi bi-stack"></i></Link></li>
            <li><a href="#"><i className="bi bi-envelope"></i></a></li>
            <li><a href="#"><i className="bi bi-trophy"></i></a></li>
            <li><a href="#"><i className="bi bi-question-circle"></i></a></li>
            <li><a href="#"><i className="bi bi-list"></i></a></li>
        </ul>
      </header>
    </div>
  );
}

export default Header;