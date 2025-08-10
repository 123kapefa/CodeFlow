import { useNavigate, Link } from "react-router-dom";
import dayjs from "dayjs";
import relativeTime from "dayjs/plugin/relativeTime";

function AuthorCard({ kind, dt, userId, name, reputation, avatarUrl }) {
  const navigate = useNavigate();
  return (
    <div
      className="post-author"
      role="button"
      tabIndex={0}
      onClick={() => navigate(`/users/${userId}`)}
      onKeyDown={(e) => (e.key === "Enter" || e.key === " ") && navigate(`/users/${userId}`)}
    >
      <small className="text-muted d-block mb-1">
        {kind} {dayjs(dt).format("MMM D, YYYY [at] HH:mm")}
      </small>

      <div className="author-card">
        {avatarUrl ? (
          <img src={avatarUrl} alt={name || "avatar"} className="author-avatar" />
        ) : (
          <div className="author-avatar-fallback">{(name?.[0] || "?").toUpperCase()}</div>
        )}

        <div className="author-info">
          <span className="author-name">{name || "—"}</span>
          {reputation != null && <span className="author-rep">{reputation}</span>}
        </div>
      </div>
    </div>
  );
}

export default AuthorCard;