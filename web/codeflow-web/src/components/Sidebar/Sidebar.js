import { NavLink } from "react-router-dom";

function Sidebar() {
  const cls = ({ isActive }) =>
    "nav-link d-flex align-items-center px-3 py-2" + (isActive ? " active" : "");

  return (
    <nav className="sidebar me-1">
      <ul className="list-unstyled m-0">
        <li>          
          <NavLink to="/home" end className={cls}>
            <i className="bi bi-house me-2" aria-hidden="true"></i>
            Home
          </NavLink>
        </li>

        <li>        
          <NavLink to="/" className={cls}>
            <i className="bi bi-question-circle me-2" aria-hidden="true"></i>
            Questions
          </NavLink>
        </li>

        <li>         
          <NavLink to="/tags" className={cls}>
            <i className="bi bi-tags me-2" aria-hidden="true"></i>
            Tags
          </NavLink>
        </li>

        <li>
          <NavLink to="/users" className={cls}>
            <i className="bi bi-people me-2" aria-hidden="true"></i>
            Users
          </NavLink>
        </li>
      </ul>
    </nav>
  );
}

export default Sidebar;
