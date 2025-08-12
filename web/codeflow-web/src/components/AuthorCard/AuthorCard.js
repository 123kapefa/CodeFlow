import { useNavigate } from "react-router-dom";
import dayjs from "dayjs";
import relativeTime from "dayjs/plugin/relativeTime";

dayjs.extend(relativeTime);

function AuthorCard({ kind, dt, userId, name, reputation, avatarUrl }) {
  const navigate = useNavigate();

  const avatarSrc = avatarUrl?.trim()
    ? avatarUrl
    : "/avatar/avatar_default.png"; // 🔹 дефолтная картинка

  return (
    <div
      className="post-author"
      role="button"
      tabIndex={0}
      onClick={() => navigate(`/users/${userId}`)}
      onKeyDown={(e) =>
        (e.key === "Enter" || e.key === " ") && navigate(`/users/${userId}`)
      }
    >
      <small className="text-muted d-block mb-1">
        {kind} {dayjs(dt).format("MMM D, YYYY [at] HH:mm")}
      </small>

      <div className="author-card">
        <img
          src={avatarSrc}
          alt={name || "avatar"}
          className="author-avatar"
        />

        <div className="author-info ms-3">
          <span className="author-name">{name || "—"}</span>
          <br />
          {reputation != null && (
            <span className="author-rep">{reputation}</span>
          )}
        </div>
      </div>
    </div>
  );
}

export default AuthorCard;
