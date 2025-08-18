import React, { useEffect, useMemo, useState } from "react";
import {
  Container,
  Spinner,
  Alert,
  ButtonGroup,
  ToggleButton,
  Pagination,
  Form,
} from "react-bootstrap";
import { useNavigate } from "react-router-dom";
import { useAuthFetch } from "../../features/useAuthFetch/useAuthFetch";

import { API_BASE } from "../../config";


export default function TagsSummaryPage({ userId }) {
  const authFetch = useAuthFetch();
  const navigate = useNavigate();

  const [loading, setLoading] = useState(true);
  const [err, setErr] = useState("");
  const [items, setItems] = useState([]);
  const [pageInfo, setPageInfo] = useState(null);

  // управление
  const [page, setPage] = useState(1);
  const pageSize = 30;
  const [orderBy, setOrderBy] = useState("QuestionsCreated"); // QuestionsCreated | AnswersWritten | Name
  const [sortDir, setSortDir] = useState(0); // 0 = Desc, 1 = Asc
  const [search, setSearch] = useState("");

  const qs = useMemo(() => {
    const q = new URLSearchParams();
    q.set("Page", page);
    q.set("PageSize", pageSize);
    q.set("OrderBy", orderBy);
    q.set("SortDirection", sortDir);
    if (search.trim()) q.set("SearchValue", search.trim());
    return q.toString();
  }, [page, pageSize, orderBy, sortDir, search]);

  useEffect(() => {
    let cancelled = false;
    (async () => {
      try {
        setLoading(true);
        setErr("");

        const res = await authFetch(
          `${API_BASE}/tags/user/${userId}/participation?${qs}`,
          { headers: { Accept: "application/json" } }
        );
        if (!res.ok) throw new Error(`HTTP ${res.status}`);
        const json = await res.json(); // { pagedInfo, value, ... }

        if (!cancelled) {
          setItems(json?.value ?? []);
          setPageInfo(json?.pagedInfo ?? null);
        }
      } catch (e) {
        if (!cancelled) setErr(e.message ?? "Failed to load tags");
      } finally {
        if (!cancelled) setLoading(false);
      }
    })();
    return () => {
      cancelled = true;
    };
  }, [authFetch, userId, qs]);

  const setSort = (by, dir) => {
    setOrderBy(by);
    setSortDir(dir);
    setPage(1);
  };

  if (loading) {
    return (
      <Container className="py-5 text-center">
        <Spinner animation="border" />
      </Container>
    );
  }

  // какой счётчик показывать справа
  const metricMode =
    orderBy === "QuestionsCreated"
      ? "questions"
      : orderBy === "AnswersWritten"
      ? "answers"
      : "posts";

  return (
    <Container className="my-2 p-0">
      {/* шапка */}
      <div className="d-flex flex-wrap justify-content-between align-items-center gap-2 mb-3">
        <div className="text-muted">
          {(pageInfo?.totalRecords ?? 0).toLocaleString()} Tags
        </div>

        <div className="d-flex align-items-center gap-2">          
          <ButtonGroup>
            <ToggleButton
              id="tags-sort-q"
              type="radio"
              variant={
                orderBy === "QuestionsCreated" && sortDir === 1
                  ? "primary"
                  : "outline-secondary"
              }
              checked={orderBy === "QuestionsCreated" && sortDir === 1}
              onChange={() => setSort("QuestionsCreated", 1)}
            >
              Questions
            </ToggleButton>
            <ToggleButton
              id="tags-sort-a"
              type="radio"
              variant={
                orderBy === "AnswersWritten" && sortDir === 1
                  ? "primary"
                  : "outline-secondary"
              }
              checked={orderBy === "AnswersWritten" && sortDir === 1}
              onChange={() => setSort("AnswersWritten", 1)}
            >
              Answers
            </ToggleButton>
            <ToggleButton
              id="tags-sort-name"
              type="radio"
              variant={
                orderBy === "Name" && sortDir === 0
                  ? "primary"
                  : "outline-secondary"
              }
              checked={orderBy === "Name" && sortDir === 0}
              onChange={() => setSort("Name", 0)}
            >
              Name
            </ToggleButton>
          </ButtonGroup>
        </div>
      </div>

      {err && <div className="alert alert-danger py-2">{err}</div>}
    
      <div className="list-group">
        {items.map((t) => {
          const qn = t.questionsCreated ?? 0;
          const an = t.answersWritten ?? 0;
          const posts = qn + an;

          let metricCount = posts;
          let metricLabel = posts === 1 ? "post" : "posts";
          if (metricMode === "questions") {
            metricCount = qn;
            metricLabel = qn === 1 ? "question" : "questions";
          } else if (metricMode === "answers") {
            metricCount = an;
            metricLabel = an === 1 ? "answer" : "answers";
          }

          return (
            <div
              key={t.tagId}
              className="list-group-item d-flex justify-content-between align-items-center"
            >
              {/* кликабельное имя тега без бейджа */}
              <h6
                className="mb-0 text-primary fw-semibold"
                style={{ cursor: "pointer" }}
                onClick={() =>
                  navigate(`/tags/${t.tagId}/questions`, {
                    state: { tagName: t.tagName },
                  })
                }
                title={`View questions with "${t.tagName}"`}
              >
                {t.tagName}
              </h6>

              {/* справа — 0 score и динамическая метрика */}
              <small className="text-muted">
                
                <span>
                  {metricCount} {metricLabel}
                </span>
              </small>
            </div>
          );
        })}

        {items.length === 0 && (
          <div className="list-group-item text-muted">Ничего не найдено</div>
        )}
      </div>

      {/* пагинация (если нужна) */}
      {pageInfo && pageInfo.totalPages > 1 && (
        <div className="d-flex justify-content-end mt-3">
          <Pagination size="sm" className="mb-0">
            <Pagination.First
              disabled={page === 1}
              onClick={() => setPage(1)}
            />
            <Pagination.Prev
              disabled={page === 1}
              onClick={() => setPage((p) => p - 1)}
            />
            <Pagination.Item active>{page}</Pagination.Item>
            <Pagination.Next
              disabled={page === pageInfo.totalPages}
              onClick={() => setPage((p) => p + 1)}
            />
            <Pagination.Last
              disabled={page === pageInfo.totalPages}
              onClick={() => setPage(pageInfo.totalPages)}
            />
          </Pagination>
        </div>
      )}
    </Container>
  );
}
