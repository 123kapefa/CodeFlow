import { Container, Row, Col, Button, Spinner } from "react-bootstrap";
import { useNavigate, Link } from "react-router-dom";
import { useEffect, useState } from "react";

import ReputationCard from "../../components/Home/ReputationCard ";
import WatchedTagsCard from "../../components/Home/WatchedTagsCard";
import QuestionCard from "../../components/QuestionCard/QuestionCard";

import { useAuth } from "../../features/Auth/AuthProvider ";
import { useAuthFetch } from "../../features/useAuthFetch/useAuthFetch";

const API = "http://localhost:5000";

export default function Home() {
  const navigate = useNavigate();

  /* 1. Хуки всегда идут первыми */
  const { user, loading } = useAuth();
  const fetchAuth = useAuthFetch();

  const [watched, setWatched] = useState(null);
  const [wErr, setWErr] = useState(null);

  /* грузим Watched Tags */
  useEffect(() => {
    if (!user) {
      navigate("/login");
      return; // ждём, пока появится профиль
    }
    let cancelled = false;

    (async () => {
      try {
        const res = await fetchAuth(`${API}/api/tags/watched/${user.userId}`);
        if (!cancelled) {
          if (res.ok) setWatched(await res.json());
          else setWErr(res.status);
        }
      } catch {
        if (!cancelled) setWErr("network");
      }
    })();

    return () => {
      cancelled = true;
    };
  }, [user, fetchAuth]); // fetchAuth уже мемоизирован

  /* хотим один раз увидеть профиль в консоли */
  useEffect(() => {
    if (user) console.log("Profile:", user);
  }, [user]);

  /* 2. Теперь идут условия с ранним return */
  if (loading) {
    return (
      <Container className="py-5 text-center">
        <Spinner animation="border" />
      </Container>
    );
  }

  /* если не залогинен, компонент ничего не рисует
     (navigate уже ушёл на /login) */
  if (!user) {
    navigate("/login");
    return null;
  }

  /* --------- заглушки --------- */

  const questions = [
    {
      id: 101,
      title:
        "TypeScript React props priority when using IntelliSense in VS Code",
      votes: 12,
      answers: 2,
      views: "2k",
      tags: ["javascript", "reactjs", "typescript", "visual-studio-code"],
      author: "Théophile Wallez 489",
      answeredAgo: "3 hours ago",
    },
    {
      id: 102,
      title: "Animate rotation of an object in JavaScript",
      votes: 1,
      answers: 1,
      views: 63,
      tags: ["javascript", "html", "css"],
      author: "Suryanarayanan S 16",
      answeredAgo: "3 hours ago",
    },
  ];
  /* ---------------------------- */

  return (
    <Container fluid="xxl" className="py-4">
      {/* шапка */}
      <Row className="align-items-center mb-4">
        <Col>
          <h2 className="mb-0">Welcome back, {user.userName}!</h2>
        </Col>
        <Col xs="auto">
          <Button
            as={Link} 
            to="/questions/ask" 
            variant="outline-primary"
          >
            Ask Question
          </Button>
        </Col>
      </Row>

      {/* две карточки одинаковой высоты */}
      <Row xs={1} lg={2} className="g-3 align-items-stretch mb-4">
        <Col>
          <ReputationCard score={user.reputation} />
        </Col>
        <Col>
          {/* разные состояния загрузки */}
          {watched === null && !wErr && (
            <WatchedTagsCard tags={[]} className="h-100">
              <div>Loading…</div>
            </WatchedTagsCard>
          )}

          {wErr && (
            <WatchedTagsCard tags={[]} className="h-100">
              <div className="text-danger">
                Failed to load Watched Tags ({wErr})
              </div>
            </WatchedTagsCard>
          )}

          {Array.isArray(watched) && (
            <WatchedTagsCard tags={watched} className="h-100" />
          )}
        </Col>
      </Row>

      {/* список вопросов */}
      <h5 className="mb-3">Interesting posts for you</h5>
      {questions.map((q) => (
        <QuestionCard key={q.id} q={q} />
      ))}
    </Container>
  );
}
