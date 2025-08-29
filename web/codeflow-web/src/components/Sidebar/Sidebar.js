import { NavLink } from "react-router-dom";

function Sidebar({ isOpen, onClose }) {
  const cls = ({ isActive }) =>
    "nav-link d-flex align-items-center px-3 py-2" +
    (isActive ? " active" : "");

  return (
    <nav className={`sidebar me-1 ${isOpen ? "is-open" : ""}`}>
      {/* Кнопка закрытия — видна только на мобильных */}
      <button className="btn btn-link d-lg-none mb-3" onClick={onClose}>
        <i className="bi bi-x-lg me-1" aria-hidden="true"></i> Close
      </button>

      <ul className="list-unstyled m-0">
        <li>
          <NavLink to="/home" end className={cls} onClick={onClose}>
            <i className="bi bi-house me-2" aria-hidden="true"></i>
            Home
          </NavLink>
        </li>

        <li>
          <NavLink to="/" className={cls} onClick={onClose}>
            <i className="bi bi-question-circle me-2" aria-hidden="true"></i>
            Questions
          </NavLink>
        </li>

        <li>
          <NavLink to="/tags" className={cls} onClick={onClose}>
            <i className="bi bi-tags me-2" aria-hidden="true"></i>
            Tags
          </NavLink>
        </li>

        <li>
          <NavLink to="/users" className={cls} onClick={onClose}>
            <i className="bi bi-people me-2" aria-hidden="true"></i>
            Users
          </NavLink>
        </li>
      </ul>
    </nav>
  );
}

export default Sidebar;
