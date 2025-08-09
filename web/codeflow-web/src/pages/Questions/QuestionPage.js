// import { useParams } from "react-router-dom";
// import { useEffect, useState } from "react";
// import {
//   Container,
//   Spinner,
//   Badge,
//   Card,
//   Button,
//   Form
// } from "react-bootstrap";

// import hljs from "highlight.js";
// import "highlight.js/styles/github.css";

// import dayjs from "dayjs";
// import relativeTime from "dayjs/plugin/relativeTime";
// dayjs.extend(relativeTime);

// export default function QuestionPage() {
//   const { id } = useParams();
//   const [data, setData] = useState(null);
//   const [loading, setLoading] = useState(true);
//   const [answerText, setAnswerText] = useState("");

//   const createdAtText = `Asked ${dayjs(question.createdAt).fromNow()}`;
// const updatedAtText = question.updatedAt
//   ? `Modified ${dayjs(question.updatedAt).fromNow()}`
//   : null;
// const viewsText = `Viewed ${question.viewsCount.toLocaleString()} times`;

//   useEffect(() => {
//     if (data) {
//       document.querySelectorAll("pre.ql-syntax").forEach((block) => {
//         hljs.highlightElement(block);
//       });
//     }
//   }, [data]);

//   const loadQuestion = () => {
//     setLoading(true);

//     fetch("http://localhost:5000/api/aggregate/get-question", {
//       method: "POST",
//       headers: {
//         "Content-Type": "application/json",
//         Accept: "application/json",
//       },
//       body: JSON.stringify({ questionId: id }),
//     })
//       .then(async (r) => {
//         if (!r.ok) throw new Error(`HTTP error ${r.status}`);
//         const text = await r.text();
//         if (!text) throw new Error("Empty response");
//         return JSON.parse(text);
//       })
//       .then((res) => {
//         setData(res);
//       })
//       .catch((err) => {
//         console.error("Ошибка при получении вопроса:", err.message);
//       })
//       .finally(() => setLoading(false));
//   };

//   const handleAnswerSubmit = (e) => {
//     e.preventDefault();
//     console.log("Отправка ответа:", answerText);
//     // TODO: отправить POST на API
//     setAnswerText("");
//   };

//   if (loading) {
//     return (
//       <div className="text-center my-5">
//         <Spinner animation="border" />
//       </div>
//     );
//   }

//   if (!data) {
//     return <Container className="my-5">Question not found</Container>;
//   }

//   const { question, answers, tags } = data;

//   return (
//     <Container className="my-4">
//       {/* Верхняя панель */}
//       <div className="d-flex justify-content-between align-items-start mb-4">
//         <h2 className="mb-0">{question.title}</h2>
//         <div className="text-muted small text-end">
//           Asked: {new Date(question.createdAt).toLocaleString()} <br />
//           Views: {question.viewsCount}
//         </div>
//       </div>

//       {/* Контент вопроса с голосами */}
//       <div className="d-flex mb-4">
//         {/* Голоса */}
//         <div className="text-center me-3">
//           <Button variant="light" size="sm" className="d-block mb-1">▲</Button>
//           <div>{question.upvotes - question.downvotes}</div>
//           <Button variant="light" size="sm" className="d-block mt-1">▼</Button>
//         </div>

//         {/* Текст вопроса */}
//         <div className="flex-grow-1">
//           <Card className="mb-2">
//             <Card.Body>
//               <div dangerouslySetInnerHTML={{ __html: question.content }} />
//             </Card.Body>
//           </Card>

//           {/* Теги */}
//           <div className="mb-3">
//             {question.questionTags.map((t) => (
//               <Badge
//                 key={t.tagId}
//                 bg="light"
//                 text="dark"
//                 className="border me-2"
//               >
//                 {tags[`tag-${t.tagId}`]?.name ?? "unknown"}
//               </Badge>
//             ))}
//           </div>
//         </div>
//       </div>

//       {/* Ответы */}
//       <h5 className="mb-3">{answers.answers.length} Answers</h5>
//       {answers.answers.map((a) => (
//         <div key={a.id} className="d-flex mb-4">
//           {/* Голоса */}
//           <div className="text-center me-3">
//             <Button variant="light" size="sm" className="d-block mb-1">▲</Button>
//             <div>{a.upvotes - a.downvotes}</div>
//             <Button variant="light" size="sm" className="d-block mt-1">▼</Button>
//           </div>

//           {/* Текст ответа */}
//           <div className="flex-grow-1">
//             <Card>
//               <Card.Body>{a.content}</Card.Body>
//             </Card>
//           </div>
//         </div>
//       ))}

//       {/* Форма ответа */}
//       <h5 className="mt-5 mb-3">Your Answer</h5>
//       <Form onSubmit={handleAnswerSubmit}>
//         <Form.Group controlId="answerText" className="mb-3">
//           <Form.Control
//             as="textarea"
//             rows={6}
//             value={answerText}
//             onChange={(e) => setAnswerText(e.target.value)}
//             placeholder="Type your answer here..."
//           />
//         </Form.Group>
//         <Button type="submit" variant="primary">
//           Post Your Answer
//         </Button>
//       </Form>
//     </Container>
//   );
// }

import { useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import { Container, Spinner, Badge, Card, Button, Form } from "react-bootstrap";

import hljs from "highlight.js";
import "highlight.js/styles/github.css";

import dayjs from "dayjs";
import relativeTime from "dayjs/plugin/relativeTime";
dayjs.extend(relativeTime);

export default function QuestionPage() {
  const { id } = useParams();
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [answerText, setAnswerText] = useState("");

  // Загрузка вопроса
  useEffect(() => {
    setLoading(true);

    fetch("http://localhost:5000/api/aggregate/get-question", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Accept: "application/json",
      },
      body: JSON.stringify({ questionId: id }),
    })
      .then(async (r) => {
        if (!r.ok) throw new Error(`HTTP error ${r.status}`);
        const text = await r.text();
        if (!text) throw new Error("Empty response");
        return JSON.parse(text);
      })
      .then((res) => {
        setData(res);
      })
      .catch((err) => {
        console.error("Ошибка при получении вопроса:", err.message);
      })
      .finally(() => setLoading(false));
  }, [id]);

  // Подсветка кода после загрузки данных
  useEffect(() => {
    if (data) {
      document.querySelectorAll("pre.ql-syntax").forEach((block) => {
        hljs.highlightElement(block);
      });
    }
  }, [data]);

  const handleAnswerSubmit = (e) => {
    e.preventDefault();
    console.log("Отправка ответа:", answerText);
    // TODO: отправить POST на API
    setAnswerText("");
  };

  if (loading) {
    return (
      <div className="text-center my-5">
        <Spinner animation="border" />
      </div>
    );
  }

  if (!data) {
    return <Container className="my-5">Question not found</Container>;
  }

  const { question, answers, tags } = data;

  // Форматируем даты и просмотры с помощью dayjs
  const createdAtText = `Asked ${dayjs(question.createdAt).fromNow()}`;
  const updatedAtText = question.updatedAt
    ? `Modified ${dayjs(question.updatedAt).fromNow()}`
    : null;
  const viewsText = `Viewed ${question.viewsCount.toLocaleString()} times`;

  return (
    <Container className="my-4">
      {/* Верхняя панель */}
      <div className="mb-4">
        <h2 className="mb-0 text-start">{question.title}</h2>
        <div className="text-muted small  text-start">
          {createdAtText}
          {updatedAtText && <> · {updatedAtText}</>}
          {" · "}
          {viewsText}
        </div>
      </div>

      {/* Контент вопроса с голосами */}
      <div className="d-flex mb-4">
        {/* Голоса */}
        <div className="text-center me-3">
          <Button variant="light" size="sm" className="d-block mb-1">
            ▲
          </Button>
          <div>{question.upvotes - question.downvotes}</div>
          <Button variant="light" size="sm" className="d-block mt-1">
            ▼
          </Button>
        </div>

        {/* Текст вопроса */}
        <div className="flex-grow-1">
          <Card className="mb-2 border-0">
            <Card.Body className="text-start">
              <div dangerouslySetInnerHTML={{ __html: question.content }} />
            </Card.Body>
          </Card>

          {/* Теги */}
          <div className="mb-3 text-start">
            {question.questionTags.map((t) => (
              <Badge
                key={t.tagId}
                bg="light"
                text="dark"
                className="border me-2"
              >
                {tags[`tag-${t.tagId}`]?.name ?? "unknown"}
              </Badge>
            ))}
          </div>
        </div>
      </div>

      {/* Ответы */}
      <h5 className="mb-3 text-start">{answers.answers.length} Answers</h5>
      {answers.answers.map((a) => (
        <div key={a.id} className="d-flex mb-4">
          {/* Голоса */}
          <div className="text-center me-3">
            <Button variant="light" size="sm" className="d-block mb-1">
              ▲
            </Button>
            <div>{a.upvotes - a.downvotes}</div>
            <Button variant="light" size="sm" className="d-block mt-1">
              ▼
            </Button>
          </div>

          {/* Текст ответа */}
          <div className="flex-grow-1">
            <Card className="border-0">
              <Card.Body className="text-start">{a.content}</Card.Body>
            </Card>
          </div>
        </div>
      ))}

      {/* Форма ответа */}
      <h5 className="mt-5 mb-3 text-start">Your Answer</h5>
      <Form onSubmit={handleAnswerSubmit}>
        <Form.Group controlId="answerText" className="mb-3">
          <Form.Control
            as="textarea"
            rows={6}
            value={answerText}
            onChange={(e) => setAnswerText(e.target.value)}
            placeholder="Type your answer here..."
          />
        </Form.Group>
        <Button type="submit" variant="primary">
          Post Your Answer
        </Button>
      </Form>
    </Container>
  );
}
