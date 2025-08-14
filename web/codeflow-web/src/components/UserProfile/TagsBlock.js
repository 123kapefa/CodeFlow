// import React from 'react';
// import { Card, Placeholder } from 'react-bootstrap';

// export default function TagsBlock({ userId }) {
//   return (
//     <Card className="mb-4">
//       <Card.Header as="h5">Tags</Card.Header>
//       <Card.Body>
//         <Placeholder as="p" animation="wave">
//           <Placeholder xs={6} /> <Placeholder xs={5} /> <Placeholder xs={4} />
//         </Placeholder>
//         <small className="text-muted">
//           TODO: популярные теги пользователя (userId = {userId})
//         </small>
//       </Card.Body>
//     </Card>
//   );
// }


import React from "react";
import { Card, Spinner, Alert } from "react-bootstrap";
import dayjs from "dayjs";
import { useUserSummary } from "../../features/useUserSummary/useUserSummary";

export default function TagsBlock({ userId, limit = 5 }) {
  const { data, loading, err } = useUserSummary(userId);
  const tags = (data?.tagsUserList ?? [])
    .slice() // копия
    .sort((a, b) =>
      (b.questionsCreated + b.answersWritten) - (a.questionsCreated + a.answersWritten)
    );

  return (
    <Card className="mb-4">
      <Card.Header as="h5" className="d-flex justify-content-between align-items-center">
        <span>Tags</span>
        <small className="text-muted">View all {data?.tagsUserList?.length ?? 0} tags</small>
      </Card.Header>

      {loading ? (
        <div className="text-center my-4"><Spinner animation="border" /></div>
      ) : err ? (
        <Alert variant="danger" className="m-3">{err}</Alert>
      ) : (
        <ul className="list-group list-group-flush">
          {tags.slice(0, limit).map(t => {
            const posts = (t.questionsCreated ?? 0) + (t.answersWritten ?? 0);
            return (
              <li key={t.tagId} className="list-group-item d-flex justify-content-between align-items-center">
                <div className="d-flex align-items-center">
                  <span className="badge bg-light text-dark border me-2">{t.tagName}</span>                 
                </div>                
                <small className="text-muted">{posts.toLocaleString()} posts</small>
              </li>
            );
          })}
          {tags.length === 0 && <li className="list-group-item text-muted">Нет тегов</li>}
        </ul>
      )}
    </Card>
  );
}