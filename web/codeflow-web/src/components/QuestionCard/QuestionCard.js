import { Card, Badge } from 'react-bootstrap';

export default function QuestionCard({ q }) {
  return (
    <Card className="shadow-sm mb-3">
      <Card.Body>
        {/* счётчики */}
        <div className="d-flex align-items-center mb-2 small text-muted">
          <span className="me-3">{q.votes} votes</span>
          <span className={`me-3 ${q.answers ? 'text-success' : ''}`}>
            {q.answers} answer{q.answers !== 1 ? 's' : ''}
          </span>
          <span>{q.views} views</span>
        </div>

        {/* заголовок */}
        <Card.Title as="h5" className="mb-2 fw-semibold">
          <a href="#question" className="link-dark text-decoration-none">
            {q.title}
          </a>
        </Card.Title>

        {/* теги */}
        <div className="mb-2">
          {q.tags.map(t => (
            <Badge key={t} bg="light" text="dark" className="border me-2">
              {t}
            </Badge>
          ))}
        </div>

        {/* автор / время */}
        <div className="small text-end text-muted">
          {q.author}&nbsp;answered&nbsp;{q.answeredAgo}
        </div>
      </Card.Body>
    </Card>
  );
}
