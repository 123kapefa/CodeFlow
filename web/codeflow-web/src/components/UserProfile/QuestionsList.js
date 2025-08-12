// import React from 'react';
// import { Card, Placeholder } from 'react-bootstrap';

// export default function QuestionsList({ userId }) {
//   return (
//     <Card className="mb-4">
//       <Card.Header as="h5">Questions</Card.Header>
//       <Card.Body>
//         <Placeholder as="p" animation="wave">
//           <Placeholder xs={12} />
//           <Placeholder xs={11} />
//           <Placeholder xs={9} />
//         </Placeholder>
//         <small className="text-muted">
//           TODO: вывести вопросы пользователя (userId = {userId})
//         </small>
//       </Card.Body>
//     </Card>
//   );
// }

import React from "react";
import { Card, Spinner, Alert } from "react-bootstrap";
import { Link } from "react-router-dom";
import dayjs from "dayjs";
import { useUserSummary } from "../../features/useUserSummary/useUserSummary";


function ViewsPill({ value }) {
  return (
    <span className="badge bg-success me-2" style={{ borderRadius: '0.25rem' }}>
      {value.toLocaleString()}
    </span>
  );
}

export default function QuestionsList({ userId, limit = 5 }) {
  const { data, loading, err } = useUserSummary(userId);
  const items = data?.questionsUserList?.value ?? [];
  const total = data?.questionsUserList?.pagedInfo?.totalRecords ?? 0;

  return (
    <Card className="mb-4">
      <Card.Header
        as="h5"
        className="d-flex justify-content-between align-items-center"
      >
        <span>Questions</span>
        <small className="text-muted">View all {total} questions</small>
      </Card.Header>

      {loading ? (
        <div className="text-center my-4">
          <Spinner animation="border" />
        </div>
      ) : err ? (
        <Alert variant="danger" className="m-3">
          {err}
        </Alert>
      ) : (
        <ul className="list-group list-group-flush">
          {items.slice(0, limit).map((q) => (
            <li
              key={q.id}
              className="list-group-item d-flex justify-content-between align-items-center"
            >
              <div className="text-truncate" style={{ maxWidth: "75%" }}>
                <ViewsPill value={q.viewsCount ?? 0} />
                <Link
                  to={`/questions/${q.id}`}
                  className="text-decoration-none"
                >
                  {q.title}
                </Link>
              </div>
              <small className="text-muted">
                {dayjs(q.createdAt).format("MMM D, YYYY")}
              </small>
            </li>
          ))}
          {items.length === 0 && (
            <li className="list-group-item text-muted">Нет вопросов</li>
          )}
        </ul>
      )}
    </Card>
  );
}
