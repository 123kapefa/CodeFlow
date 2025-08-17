import React, { useEffect, useState, useCallback } from "react";
import { Container, Spinner, ButtonGroup, ToggleButton } from "react-bootstrap";
import QuestionSummaryCard from "../../components/QuestionCard/QuestionSummaryCard"; 
import { useAuthFetch } from "../../features/useAuthFetch/useAuthFetch";

import { API_BASE } from "../../config";


export default function QuestionsSummaryPage({ userId }) {
  const authFetch = useAuthFetch();      // добавит Bearer + рефреш при 401
  const [loading, setLoading] = useState(true);
  const [questions, setQuestions] = useState([]);
  const [pageInfo, setPageInfo] = useState(null);
  const [tagsMap, setTagsMap] = useState({});

  // сортировка и пагинация
  const [orderBy, setOrderBy] = useState("CreatedAt");     // CreatedAt | AnswersCount | Views | Score (если поддерживается)
  const [sortDir, setSortDir] = useState("Descending");    // "Descending" | "Ascending"
  const [page, setPage] = useState(1);
  const pageSize = 30;

  const load = useCallback(async () => {
    setLoading(true);
    try {
      // ВНИМАНИЕ: агрегатор ждёт page/pageSize/orderBy/sortDirection (нижний регистр)
      const url =
        `${API_BASE}/aggregate/get-questions-summary/${userId}` +
        `?page=${page}&pageSize=${pageSize}&orderBy=${orderBy}&sortDirection=${sortDir}`;

      const res = await authFetch(url, {
        method: "POST",
        headers: { Accept: "application/json" },
      });
      if (!res.ok) throw new Error(`HTTP ${res.status}`);

      const json = await res.json();
      // ожидаем { questionsList: { pagedInfo, value }, tagsList: [...] }
      const list = json?.questionsList ?? {};
      setQuestions(list?.value ?? []);
      setPageInfo(list?.pagedInfo ?? null);

      const map = Object.create(null);
      for (const t of json?.tagsList ?? []) map[t.id] = t.name;
      setTagsMap(map);
    } finally {
      setLoading(false);
    }
  }, [authFetch, userId, page, pageSize, orderBy, sortDir]);

  useEffect(() => { load(); }, [load]);

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
          {(pageInfo?.totalRecords ?? 0).toLocaleString()} questions
        </div>

        <ButtonGroup>
          <ToggleButton
            id="sort-answers"
            type="radio"
            variant={orderBy === "AnswersCount" ? "primary" : "outline-secondary"}
            checked={orderBy === "AnswersCount"}
            onChange={() => setSort("AnswersCount", "Descending")}
          >
            Answered
          </ToggleButton>
          <ToggleButton
            id="sort-newest"
            type="radio"
            variant={orderBy === "CreatedAt" ? "primary" : "outline-secondary"}
            checked={orderBy === "CreatedAt"}
            onChange={() => setSort("CreatedAt", "Descending")}
          >
            Newest
          </ToggleButton>
        </ButtonGroup>
      </div>

      <div className="list-group">
        {questions.length === 0 ? (
          <div className="list-group-item text-muted">Нет вопросов</div>
        ) : (
          questions.map((q) => (
            <QuestionSummaryCard key={q.id} q={q} tagsMap={tagsMap} />
          ))
        )}
      </div>

      {/* при необходимости добавь пагинацию ниже */}
    </Container>
  );
}