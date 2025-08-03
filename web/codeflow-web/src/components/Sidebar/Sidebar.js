import React from "react";
import { Link } from 'react-router-dom';

import '../../App.css'


function Sidebar() {
  return (
    <nav className="sidebar">
      <ul>
        <li><Link to="/"><i className="bi bi-house"></i> Home</Link></li>
        <li><Link to="/questions">Questions</Link></li>
        <li><Link to="/tags">Tags</Link></li>
        <li><Link to="/users">Users</Link></li>
      </ul>
    </nav>
  );
}

export default Sidebar;