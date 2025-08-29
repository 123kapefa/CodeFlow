import React from 'react';
import { Col, Image, Badge } from 'react-bootstrap';
import PropTypes from 'prop-types';
import { Link } from 'react-router-dom';

const UserCard = ({ user }) => {  
  const avatarSrc =
    user.avatarUrl && user.avatarUrl.trim()
      ? user.avatarUrl                    // то, что пришло с сервера
      : '/avatar/avatar_default.png';     // локальная заглушка

  return (
   <Col xl={3} lg={3} md={4} sm={6} xs={12} className="mb-4">
      {/* ссылка охватывает ВЕСЬ контент карточки */}
      <Link
        to={`/users/${user.userId}`}         
        className="text-decoration-none text-reset"
      >
        <div className="d-flex">
          <Image
            src={avatarSrc}
            rounded
            width={64}
            height={64}
            className="me-3 flex-shrink-0"
          />

          <div className="d-flex flex-column">
            <span className="fw-semibold">{user.userName}</span>
            <span className="text-muted small">
              Reputation: {user.reputation}
            </span>

            {user.location && (
              <span className="text-muted small">{user.location}</span>
            )}

            {user.tags?.length > 0 && (
              <div className="mt-1">
                {user.tags.slice(0, 3).map((tag) => (
                  <Badge
                    key={tag}
                    bg="light"
                    text="secondary"
                    className="me-1 mb-1"
                  >
                    {tag}
                  </Badge>
                ))}
              </div>
            )}
          </div>
        </div>
      </Link>
    </Col>
  );
};

UserCard.propTypes = {
  user: PropTypes.shape({
    userName:   PropTypes.string.isRequired,
    avatarUrl:  PropTypes.string,
    reputation: PropTypes.number.isRequired,
    location:   PropTypes.string,
    tags:       PropTypes.arrayOf(PropTypes.string),
  }).isRequired,
};

export default UserCard;