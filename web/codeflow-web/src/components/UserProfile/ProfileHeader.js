import React from 'react';
import { Row, Col, Image, Badge } from 'react-bootstrap';
import { Globe, GeoAlt, Github, Calendar } from 'react-bootstrap-icons';
import { formatMemberSince, formatLastSeen } from '../../features/Date/dateUtils';

export default function ProfileHeader({ profile }) {
  const {
    avatarUrl, userName, createdAt, lastVisitAt,
    location, websiteUrl, gitHubUrl, aboutMe, reputation,
  } = profile;

  return (
    <Row className="mb-4 align-items-start">
      {/* аватар */}
      <Col md="auto" className="text-center pe-0">
        <Image
          src={avatarUrl?.trim() || '/avatar/avatar_default.png'}
          width={128} height={128} className="mb-3"
        />
      </Col>

      {/* инфо */}
      <Col className="ps-3 text-start">
        <h3 className="mb-1">{userName}</h3>

        <div className="text-muted small mb-2">
          <Calendar className="me-1" />
          {formatMemberSince(createdAt)} • {formatLastSeen(lastVisitAt)}
        </div>

        <div className="d-flex flex-wrap align-items-center mb-2">
          {websiteUrl && (
            <a
              href={`https://${websiteUrl}`} target="_blank" rel="noreferrer"
              className="d-inline-flex align-items-center me-3 text-decoration-none text-muted"
            >
              <Globe className="me-1" />
              {websiteUrl.replace(/^https?:\/\//, '')}
            </a>
          )}

          {gitHubUrl && (
            <a
              href={gitHubUrl} target="_blank" rel="noreferrer"
              className="d-inline-flex align-items-center me-3 text-decoration-none text-muted"
            >
              <Github className="me-1" /> GitHub
            </a>
          )}

          {location && (
            <span className="d-inline-flex align-items-center text-muted">
              <GeoAlt className="me-1" /> {location}
            </span>
          )}
        </div>

        

        <Badge bg="success" className="mt-2">
          {reputation} reputation
        </Badge>
      </Col>
    </Row>
  );
}