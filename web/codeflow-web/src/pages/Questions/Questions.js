import { useEffect, useState } from "react";
import {
  Container,
  Row,
  Col,
  Button,
  Pagination,
  ButtonGroup,
  ToggleButton,
  Spinner,
} from "react-bootstrap";
import {
  Link,
  useLocation,
  useParams,
  useSearchParams,
} from "react-router-dom";

import QuestionCard from "../../components/QuestionCard/QuestionCard";

function Questions() {
  /* ───────── URL-параметры ───────── */
  const { tagId } = useParams(); // undefined или id тега
  const location = useLocation();
  const initialTagName = location.state?.tagName ?? null;
  const [qs, setQs] = useSearchParams();

  const page = parseInt(qs.get("page") ?? "1", 10);
  const orderBy = qs.get("orderBy") ?? "AnswersCount";
  const sortDir = parseInt(qs.get("sortDir") ?? "1", 10); // проверь соответствие на бэке

  /* ───────── состояние ───────── */
  const [items, setItems] = useState([]);
  const [pageInfo, setInfo] = useState(null);
  const [loading, setLoading] = useState(true);
  const [currentTag, setCurrentTag] = useState(null);

  // 1) Берём имя тега из state, если пришли со страницы Tags
  useEffect(() => {
    if (!tagId) {
      setCurrentTag(null);
      return;
    }
    if (typeof initialTagName === "string" && initialTagName.trim()) {
      setCurrentTag(initialTagName.trim());
    } else {
      setCurrentTag(null);
    }
  }, [tagId, initialTagName]);

  /* ───────── загрузка ───────── */
  useEffect(() => {
    let isCancelled = false;

    const fetchData = async () => {
      try {
        setLoading(true);

        const url =
          `http://localhost:5000/api/aggregate/get-questions?page=${page}` +
          `&pageSize=30&orderBy=${encodeURIComponent(
            orderBy
          )}&sortDirection=${sortDir}` +
          (tagId ? `&tagId=${tagId}` : "");

        const r = await fetch(url, {
          method: "POST",
          headers: { Accept: "application/json" },
        });
        if (!r.ok) throw new Error(`HTTP error ${r.status}`);

        const res = await r.json();

        const questionsList = res?.questionsList ?? res?.questions ?? {};
        const tagsList = res?.tagsList ?? res?.tags ?? [];
        const usersList = res?.usersList ?? res?.users ?? [];

        // Кэш по тегам и пользователям
        const tagMap = new Map((tagsList ?? []).map((t) => [t.id, t.name]));
        const userMap = new Map((usersList ?? []).map((u) => [u.userId, u]));

        // Если имени тега нет в state — попробуем достать из tagsList по id
        if (tagId && !currentTag) {
          const t = (tagsList ?? []).find(
            (t) => String(t.id) === String(tagId)
          );
          if (t?.name) setCurrentTag(t.name);
        }

        const list = questionsList?.value ?? [];
        const mappedItems = list.map((q) => {
          const tagItems =
            (q.questionTags ?? []).map((t) => ({
              id: t.tagId,
              name: tagMap.get(t.tagId) ?? `tag-${t.tagId}`,
            })) ?? [];

          const author = userMap.get(q.userId);
          return {
            id: q.id,
            title: q.title,
            votes: (q.upvotes ?? 0) - (q.downvotes ?? 0),
            answers: q.answersCount ?? 0,
            views: q.viewsCount ?? 0,
            tags: tagItems.map((x) => x.name),
            tagItems, // для кликабельных ссылок на теги
            isClosed: !!q.isClosed,
            author: author?.username ?? "unknown",
            authorAvatar: author?.avatarUrl ?? null,
            authorReputation: author?.reputation ?? 0,
            answeredAgo: new Date(q.createdAt).toLocaleDateString(),
            content: q.content ?? null,
          };
        });

        if (!isCancelled) {
          setItems(mappedItems);
          setInfo(questionsList?.pagedInfo ?? null);
        }
      } catch (err) {
        console.error("Ошибка при получении вопросов:", err?.message ?? err);
      } finally {
        if (!isCancelled) setLoading(false);
      }
    };

    fetchData();
    return () => {
      isCancelled = true;
    };
    // не добавляем currentTag сюда, чтобы не было лишних перезапросов
  }, [page, orderBy, sortDir, tagId]); // eslint-disable-line react-hooks/exhaustive-deps

  // Хелперы для обновления query-строки (не мутируем исходный qs)
  const setPageQuery = (p) => {
    const next = new URLSearchParams(qs);
    next.set("page", String(p));
    setQs(next);
  };

  const setSort = (field, dir) => {
    const next = new URLSearchParams(qs);
    next.set("orderBy", field);
    next.set("sortDir", String(dir));
    next.set("page", "1");
    setQs(next);
  };

  return (
    <Container className="my-4">
      <Row className="align-items-center mb-5">
        <Col>
          <h2 className="mb-3">
            {tagId ? (
              <>Questions tagged [{currentTag?.trim() ?? ""}]</>
            ) : (
              "All questions"
            )}
          </h2>
        </Col>
        <Col xs="auto">
          <Button as={Link} to="/questions/ask" variant="outline-primary">
            Ask Question
          </Button>
        </Col>
      </Row>

      <div className="d-flex justify-content-end mb-3">
        <ButtonGroup className="mb-3">
          <ToggleButton
            id="sort-answers"
            type="radio"
            variant={
              orderBy === "AnswersCount" ? "primary" : "outline-secondary"
            }
            checked={orderBy === "AnswersCount"}
            onChange={() => setSort("AnswersCount", 1)} // Desc
          >
            Answered
          </ToggleButton>

          <ToggleButton
            id="sort-new"
            type="radio"
            variant={orderBy === "CreatedAt" ? "primary" : "outline-secondary"}
            checked={orderBy === "CreatedAt"}
            onChange={() => setSort("CreatedAt", 1)} // Desc
          >
            Newest
          </ToggleButton>
        </ButtonGroup>
      </div>

      {loading ? (
        <div className="text-center my-5">
          <Spinner animation="border" />
        </div>
      ) : (
        <>
          <Row>
            {items.map((q) => (
              <Col key={q.id} xs={12}>
                <QuestionCard q={q} />
              </Col>
            ))}
          </Row>

          {pageInfo && pageInfo.totalPages > 0 && (
            <div className="d-flex justify-content-center">
              <Pagination>
                <Pagination.First
                  disabled={page === 1}
                  onClick={() => setPageQuery(1)}
                />
                <Pagination.Prev
                  disabled={page === 1}
                  onClick={() => setPageQuery(page - 1)}
                />
                {[...Array(pageInfo.totalPages)].map((_, i) => (
                  <Pagination.Item
                    key={i + 1}
                    active={i + 1 === page}
                    onClick={() => setPageQuery(i + 1)}
                  >
                    {i + 1}
                  </Pagination.Item>
                ))}
                <Pagination.Next
                  disabled={page === pageInfo.totalPages}
                  onClick={() => setPageQuery(page + 1)}
                />
                <Pagination.Last
                  disabled={page === pageInfo.totalPages}
                  onClick={() => setPageQuery(pageInfo.totalPages)}
                />
              </Pagination>
            </div>
          )}
        </>
      )}
    </Container>
  );
}

export default Questions;
