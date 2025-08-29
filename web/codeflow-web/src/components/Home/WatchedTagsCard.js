import { Card, Badge } from 'react-bootstrap';
import { Link } from "react-router-dom";

export default function WatchedTagsCard({ tags = [], className = '' }) {
  return (
    <Card className={`h-100 shadow-sm ${className}`}>
      <Card.Header className="bg-light d-flex justify-content-between align-items-center text-uppercase fw-bold small">
        <span>Watched tags</span>
        <a href="#all-tags" className="small">
          See all
        </a>
      </Card.Header>

      <Card.Body>
        {tags.length ? (
          <div className="d-flex flex-wrap justify-content-center">
            {tags.map((t) => (
              <Badge
                key={t.id}
                as={Link}
                to={`/tags/${t.tagId}/questions`}
                state={{ tagName: t.tagName ?? t.name }}
                bg="secondary"
                className="me-2 mb-2 tag-badge text-decoration-none"
                title={`Show questions tagged [${t.tagName ?? t.name}]`}
              >
                {t.tagName ?? t.name}
              </Badge>
            ))}
          </div>
        ) : (
          <span className="text-muted">No tags yet</span>
        )}
      </Card.Body>
    </Card>
  );
}
