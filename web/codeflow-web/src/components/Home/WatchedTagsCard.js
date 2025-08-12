import { Card, Badge } from 'react-bootstrap';

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
          tags.map(t => (
            <Badge key={t.id} bg="secondary" className="me-2 mb-2 tag-badge">
              {t.tagName}
            </Badge>
          ))
        ) : (
          <span className="text-muted">No tags yet</span>
        )}
      </Card.Body>
    </Card>
  );
}
