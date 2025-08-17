import { Card } from 'react-bootstrap';

export default function ReputationCard({ score = 1, className = '' }) {
  return (
    <Card className={`h-100 shadow-sm ${className}`}>
      <Card.Body>
        <Card.Title as="h6" className="text-uppercase fw-bold small mb-2">
          Reputation
        </Card.Title>
        <h3 className="fw-bold mb-2">{score}</h3>
        <small className="text-muted">
          Earn reputation by&nbsp;
          <a href="/help">asking</a>, <a href="/help">answering</a> &amp;&nbsp;
          <a href="/help">editing</a>.
        </small>
      </Card.Body>
    </Card>
  );
}
