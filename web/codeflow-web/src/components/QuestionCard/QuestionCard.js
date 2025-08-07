import { Card, Badge } from "react-bootstrap";
import { Link } from "react-router-dom";
import { CheckLg } from "react-bootstrap-icons";

function AnswersPill({ answers, isClosed }) {
  const hasAnswers = (answers ?? 0) > 0;
  let cls = "answers-pill none";
  if (isClosed) cls = "answers-pill accepted";
  else if (hasAnswers) cls = "answers-pill outlined";

  return (
    <div className={cls}>
      {isClosed && <CheckLg />}
      <span>{answers ?? 0}</span>&nbsp;
      <span>{(answers ?? 0) === 1 ? "answer" : "answers"}</span>
    </div>
  );
}

export default function QuestionCard({ q }) {
  if (!q) return null;

  const votes = q.votes ?? 0;
  const answers = q.answers ?? 0;
  const views = q.views ?? 0;

  // Предпочитаем q.tagItems [{id,name}], иначе fallback к q.tags (имена)
  const tagItems =
    Array.isArray(q.tagItems) && q.tagItems.length
      ? q.tagItems
      : Array.isArray(q.tags)
      ? q.tags.map((name, i) => ({ id: null, name }))
      : [];

  return (
    <Card className="shadow-sm mb-3 border-0">
      <Card.Body className="p-3">
        <div className="d-flex">
          {/* левая колонка со счетчиками */}
          <div className="text-center me-5" style={{ width: 90 }}>
            <div className="small text-muted">{votes} votes</div>

            <div className="mt-2">
              <AnswersPill answers={answers} isClosed={q.isClosed} />
            </div>

            <div className="small text-muted mt-2">{views} views</div>
        </div>

          {/* правая колонка */}
          <div className="flex-grow-1">
            <Card.Title as="h5" className="mb-1 fw-semibold">
              <Link
                to={`/questions/${q.id}`}
                className="link-primary text-decoration-none"
              >
            {q.title}
              </Link>
        </Card.Title>

            {q.content && (
              <div
                className="text-muted small mb-2"
                style={{ maxHeight: 40, overflow: "hidden" }}
                dangerouslySetInnerHTML={{ __html: q.content }}
              />
            )}

            {/* кликабельные теги */}
        <div className="mb-2">
              {tagItems.map((t, i) => (
                <Badge
                  key={t.id ?? `${t.name}-${i}`}
                  as={Link}
                  to={t.id ? `/tags/${t.id}/questions` : `/tags/search`}
                  state={t.name ? { tagName: t.name } : undefined}
                  bg="light"
                  text="dark"
                  className="border me-2 tag-badge"
                  style={{ fontSize: "0.85rem", cursor: "pointer" }}
                  title={`tag: ${t.name}`}
                >
                  {t.name}
            </Badge>
          ))}
        </div>

            <div className="small text-muted text-end">
              asked {q.answeredAgo}
            </div>
          </div>
        </div>
      </Card.Body>
    </Card>
  );
}
