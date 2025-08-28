import React, { useCallback, useEffect, useState } from "react";
import { Container, Spinner, ButtonGroup, ToggleButton } from "react-bootstrap";

import QuestionSummaryCard from "../../components/QuestionCard/QuestionSummaryCard";
import { useAuthFetch } from "../../features/useAuthFetch/useAuthFetch";

import { API_BASE } from "../../config";

export default function AnswersSummaryPage({ userId }) {
  const authFetch = useAuthFetch();
  const [loading, setLoading] = useState(true);
  const [items, setItems] = useState([]);
  const [pageInfo, setPageInfo] = useState(null);
  const [tagsMap, setTagsMap] = useState({});

  // сортировка и пагинация
  const [orderBy, setOrderBy] = useState("CreatedAt");
  const [sortDir, setSortDir] = useState("Descending");
  const [page, setPage] = useState(1);
  const pageSize = 30;

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const url =
        `${API_BASE}/aggregate/get-answers-summary/${userId}` +
        `?page=${page}&pageSize=${pageSize}&orderBy=${orderBy}&sortDirection=${sortDir}`;

      const res = await authFetch(url, {
        method: "POST",
        headers: { Accept: "application/json" },
      });
      if (!res.ok) throw new Error(`HTTP ${res.status}`);
      const json = await res.json();

      // данные в questionsList
      const raw = json?.questionsList ?? [];
      const items = Array.isArray(raw) ? raw : raw?.value ?? [];
      setItems(items);

      const info = Array.isArray(raw)
        ? {
            pageNumber: page,
            pageSize,
            totalPages: 1,
            totalRecords: raw.length,
          }
        : raw?.pagedInfo ?? null;
      setPageInfo(info);
      // теги в tagsList
      const map = Object.create(null);
      for (const t of json?.tagsList ?? []) map[t.id] = t.name;
      setTagsMap(map);
    } finally {
      setLoading(false);
    }
  }, [userId, page, pageSize, orderBy, sortDir]);

  useEffect(() => {
    load();
  }, [load]);

  if (loading) {
    return (
      <Container className="py-5 text-center">
        <Spinner animation="border" />
      </Container>
    );
  }

  const setSort = (by, dir) => {
    setOrderBy(by);
    setSortDir(dir);
    setPage(1);
  };

  return (
    <Container className="my-2 p-0">
      {/* заголовок: слева количество, справа сортировки */}
      <div className="d-flex justify-content-between align-items-center mb-3">
        <div className="text-muted">
          {(pageInfo?.totalRecords ?? 0).toLocaleString()} answers
        </div>

        <ButtonGroup>
          <ToggleButton
            id="answers-sort-newest"
            type="radio"
            variant={orderBy === "CreatedAt" ? "primary" : "outline-secondary"}
            checked={orderBy === "CreatedAt"}
            onChange={() => setSort("CreatedAt", "Descending")}
          >
            Newest
          </ToggleButton>
          <ToggleButton
            id="answers-sort-most"
            type="radio"
            variant={
              orderBy === "AnswersCount" ? "primary" : "outline-secondary"
            }
            checked={orderBy === "AnswersCount"}
            onChange={() => setSort("AnswersCount", "Descending")}
          >
            Most answers
          </ToggleButton>
        </ButtonGroup>
      </div>

      <div className="list-group">
        {items.length === 0 ? (
          <div className="list-group-item text-muted">Пока нет ответов</div>
        ) : (
          items.map((q) => (
            <QuestionSummaryCard key={q.id} q={q} tagsMap={tagsMap} />
          ))
        )}
      </div>
    </Container>
  );
}
