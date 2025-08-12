import React from "react";
import { Link } from "react-router-dom";
import dayjs from "dayjs";
import "./QuestionSummaryCard.css";

function formatCompact(n) {
  if (n == null) return "0";
  if (n >= 1_000_000)
    return (n / 1_000_000).toFixed(1).replace(/\.0$/, "") + "m";
  if (n >= 1_000) return (n / 1_000).toFixed(1).replace(/\.0$/, "") + "k";
  return n.toString();
}

export default function QuestionSummaryCard({ q, tagsMap }) {
  const votes = (q.upvotes ?? 0) - (q.downvotes ?? 0);
  const answers = q.answersCount ?? 0;
  const views = q.viewsCount ?? 0;

  //  если вопрос закрыт ИЛИ есть acceptedAnswerId
  const solved = q.isClosed === true || !!q.acceptedAnswerId;

  const tagName = (t) =>
    t.tagName ?? (tagsMap ? tagsMap[t.tagId] : null) ?? `tag-${t.tagId}`;

  return (
    <div className="qso-item list-group-item">
      <div className="qso-left">
        <span className="qso-metric text-muted">
          <strong>{formatCompact(votes)}</strong> votes
        </span>

        <span
          className={`badge qso-badge-ans ${
            solved ? "bg-success" : "bg-light text-dark border"
          }`}
        >
          {solved ? "✓ " : ""}
          {answers} {answers === 1 ? "answer" : "answers"}
        </span>

        <span className="qso-metric qso-views">
          <strong>{formatCompact(views)}</strong> views
        </span>
      </div>

      <div className="qso-main">
        <Link to={`/questions/${q.id}`} className="qso-title">
          {q.title}
        </Link>

        <div className="qso-tags">
          {(q.questionTags ?? []).map((t) => (
            <span
              key={t.tagId}
              className="badge bg-light text-dark border qso-tag"
            >
              {tagName(t)}
            </span>
          ))}
        </div>
      </div>

      <div className="qso-right">
        <small className="text-muted">
          asked {dayjs(q.createdAt).format("MMM D, YYYY [at] H:mm")}
        </small>
      </div>
    </div>
  );
}
