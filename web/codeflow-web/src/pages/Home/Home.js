import { Container, Row, Col, Button, Spinner } from "react-bootstrap";
import { useNavigate, Link } from "react-router-dom";
import { useEffect, useState } from "react";

import ReputationCard from "../../components/Home/ReputationCard ";
import WatchedTagsCard from "../../components/Home/WatchedTagsCard";
import QuestionCard from "../../components/QuestionCard/QuestionCard";

import { useAuth } from "../../features/Auth/AuthProvider ";
import { useAuthFetch } from "../../features/useAuthFetch/useAuthFetch";

import "./Home.css";

import { API_BASE } from "../../config";

function timeAgo(iso) {
  if (!iso) return "";
  const diff = Date.now() - new Date(iso).getTime();
  const s = Math.max(1, Math.floor(diff / 1000));
  const m = Math.floor(s / 60);
  const h = Math.floor(m / 60);
  const d = Math.floor(h / 24);
  if (d > 0) return `${d} ${d === 1 ? "day" : "days"} ago`;
  if (h > 0) return `${h} ${h === 1 ? "hour" : "hours"} ago`;
  if (m > 0) return `${m} ${m === 1 ? "minute" : "minutes"} ago`;
  return `${s} ${s === 1 ? "second" : "seconds"} ago`;
}

export default function Home() {
  const navigate = useNavigate();

  const { user, loading } = useAuth();
  const fetchAuth = useAuthFetch();

  // Watched tags
  const [watched, setWatched] = useState(null);
  const [wErr, setWErr] = useState(null);

  // Recommended
  const [recLoading, setRecLoading] = useState(true);
  const [recError, setRecError] = useState(null);
  const [recommended, setRecommended] = useState([]); // уже в формате QuestionCard

  /* грузим Watched Tags */
  useEffect(() => {
    if (!user) {
      navigate("/login");
      return;
    }
    let cancelled = false;

    (async () => {
      try {
        const res = await fetchAuth(`${API_BASE}/tags/watched/${user.userId}`);
        if (!cancelled) {
          if (res.ok) setWatched(await res.json());
          else setWErr(res.status);
        }
      } catch {
        if (!cancelled) setWErr("network");
      }
    })();

    return () => { cancelled = true; };
  }, [user, fetchAuth, navigate]);

  /* грузим Recommended */
  useEffect(() => {
    if (!user) return;
    let cancelled = false;

    const url = `${API_BASE}/aggregate/recommended/${user.userId}` +
                `?page=1&pageSize=40&orderBy=CreatedAt&sortDirection=Ascending`;

    (async () => {
      try {
        setRecLoading(true);
        setRecError(null);

        const res = await fetchAuth(url, { method: "GET" });
        if (!res.ok) {
          if (!cancelled) setRecError(`HTTP ${res.status}`);
          return;
        }

        const data = await res.json();
        if (cancelled) return;

        // Быстрые словари для поиска автора/тегов
        const usersById = new Map(
          (data.users ?? []).map(u => [u.userId, u])
        );
        const tagsById = new Map(
          (data.tags ?? []).map(t => [t.id, t])
        );

        const mapped = (data.items ?? []).map(item => {
          const q = item.question;
          const author = usersById.get(q.userId);

          // Собираем теги из questionTags.tagId -> tags.name
          const tagItems = Array.isArray(q.questionTags)
            ? q.questionTags
                .map(t => {
                  const meta = tagsById.get(t.tagId);
                  return meta ? { id: meta.id, name: meta.name } : null;
                })
                .filter(Boolean)
            : [];

          return {
            id: q.id,
            title: q.title,
            content: q.content,
            votes: (q.upvotes ?? 0) - (q.downvotes ?? 0),
            answers: q.answersCount ?? 0,
            views: q.viewsCount ?? 0,
            isClosed: q.isClosed ?? false,
            tagItems,
            authorId: author?.userId ?? q.userId,
            author: author
              ? `${author.username}`
              : "unknown",
            authorReputation: author?.reputation ?? 0,
            authorAvatar: author?.avatarUrl ?? "",
            createdAt: q.createdAt
          };
        });

        setRecommended(mapped);
      } catch (e) {
        if (!cancelled) setRecError("network");
      } finally {
        if (!cancelled) setRecLoading(false);
      }
    })();

    return () => { cancelled = true; };
  }, [user, fetchAuth]);

  /* 2. Условия с ранним return */
  if (loading) {
    return (
      <Container className="py-5 text-center">
        <Spinner animation="border" />
      </Container>
    );
  }

  if (!user) {
    navigate("/login");
    return null;
  }

  return (
    <Container fluid="xxl" className="py-4 home-page">
      {/* шапка */}
      <Row className="align-items-center mb-5">
        <Col>
          <h2 className="mb-0 text-start">Welcome back, {user.userName}!</h2>
        </Col>
        <Col xs="auto mb-2">
          <Button as={Link} to="/questions/ask" variant="outline-primary">
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
          {/* Watched Tags состояния */}
          {watched === null && !wErr && (
            <WatchedTagsCard tags={[]} className="h-100 ">
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

      {/* список рекомендаций */}
      <h5 className="mb-3">Interesting posts for you</h5>

      {recLoading && (
        <div className="py-3 text-center">
          <Spinner animation="border" />
        </div>
      )}

      {recError && !recLoading && (
        <div className="text-danger mb-3">
          Failed to load recommendations ({recError})
        </div>
      )}

      {!recLoading && !recError && recommended.length === 0 && (
        <div className="text-muted">No recommendations yet.</div>
      )}

      {!recLoading && !recError && recommended.map(q => (
        <QuestionCard key={q.id} q={q} />
      ))}
    </Container>
  );
}