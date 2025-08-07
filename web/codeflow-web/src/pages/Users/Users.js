import React, { useState, useEffect, useMemo } from "react";
import {
  Container,
  Row,
  Col,
  ButtonGroup,
  ToggleButton,
  Form,
  Spinner,
  Alert,
  Pagination
} from "react-bootstrap";
import UserCard from "../../components/UserCard/UserCard";

const PAGE_SIZE = 36;
const API_URL = "http://localhost:5000/api/users";

export default function UsersPage() {
  /* ---------------------------- state ---------------------------- */
  const [users, setUsers] = useState([]);
  const [page, setPage] = useState(1);
  const [pagesTotal, setPagesTotal] = useState(1);
  const [filter, setFilter] = useState("");
  const [sort, setSort] = useState("reputation"); // reputation | new
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  /* ------------------------ query string ------------------------- */
  const query = useMemo(() => {
    const q = new URLSearchParams();

    // обязательные параметры пагинации
    q.append("Page", page);
    q.append("PageSize", PAGE_SIZE);

    // (если нужен поиск по имени — передайте свой параметр; пример:)
    if (filter.trim()) q.append("Filter", filter.trim());

    // сортировка
    if (sort === "reputation") {
      q.append("OrderBy", "Reputation");
      q.append("SortDirection", "1"); // 1 = DESC в вашей схеме
    } else {
      q.append("OrderBy", "CreatedAt");
      q.append("SortDirection", "1");
    }
    return q.toString();
  }, [page, filter, sort]);

  /* --------------------------- fetch ----------------------------- */
  useEffect(() => {
    const ctrl = new AbortController();
    (async () => {
      setLoading(true);
      try {     
        const res = await fetch(`${API_URL}?${query}`, { signal: ctrl.signal });

        if (!res.ok) throw new Error(`HTTP ${res.status}`);

        const dto = await res.json();

        setUsers(dto.value);
        setPagesTotal(dto.pagedInfo.totalPages || 1);
        setError(null);
      } catch (e) {
        if (e.name !== "AbortError") setError(e.message);
      } finally {
        setLoading(false);
      }
    })();
    return () => ctrl.abort();
  }, [query]);

  /* ----------------------- pagination ---------------------------- */
  const Pager = () => {
    if (pagesTotal <= 1) return null;

    // строим диапазон 1 … 2 3 4 5 … N
    const items = [];
    const window = 5;
    const head = Math.min(pagesTotal, window);

    // «prev»
    items.push(
      <Pagination.Prev
        key="prev"
        disabled={page === 1}
        onClick={() => setPage((p) => Math.max(1, p - 1))}
      />
    );

    // первые страницы
    for (let p = 1; p <= head; p++) {
      items.push(
        <Pagination.Item key={p} active={p === page} onClick={() => setPage(p)}>
          {p}
        </Pagination.Item>
      );
    }

    // многоточие + последняя страница
    if (pagesTotal > window) {
      items.push(<Pagination.Ellipsis key="ellipsis" disabled />);
      items.push(
        <Pagination.Item
          key={pagesTotal}
          active={page === pagesTotal}
          onClick={() => setPage(pagesTotal)}
        >
          {pagesTotal}
        </Pagination.Item>
      );
    }

    // «next»
    items.push(
      <Pagination.Next
        key="next"
        disabled={page === pagesTotal}
        onClick={() => setPage((p) => Math.min(pagesTotal, p + 1))}
      />
    );

    return <Pagination className="justify-content-center">{items}</Pagination>;
  };

  /* --------------------------- ui ---------------------------- */
  return (
    <Container className="py-4">
      <h2 className="mb-4">Users</h2>

      {/* filter & sort */}
      <Row className="g-2 mb-3">
        <Col md="auto">
          <Form className="me-3" style={{ maxWidth: "300px" }}>
            <Form.Control
              type="text"
              placeholder="Filter by user"
              value={filter}
              onChange={(e) => {
                setFilter(e.target.value);
                setPage(1);
              }}
            />
          </Form>
        </Col>

        <Col md={3} className="ms-auto">
          <ButtonGroup>
            <ToggleButton
              id="reputation"
              type="radio"
              variant={sort === "reputation" ? "primary" : "outline-secondary"}
              checked={sort === "reputation"}
              onChange={() => {
                setSort("reputation");
                setPage(1);
              }}
            >
              Reputation
            </ToggleButton>

            <ToggleButton
              id="new"
              type="radio"
              variant={sort === "new" ? "primary" : "outline-secondary"}
              checked={sort === "new"}
              onChange={() => {
                setSort("new");
                setPage(1);
              }}
            >
              New users
            </ToggleButton>
          </ButtonGroup>
        </Col>
      </Row>

      {/* grid */}
      {loading && (
        <div className="text-center my-5">
          <Spinner animation="border" role="status" />
        </div>
      )}

      {error && <Alert variant="danger">Ошибка: {error}</Alert>}

      <Row>
        {users.map((u) => (
          <UserCard key={u.userName} user={u} />
        ))}
      </Row>

      {/* pagination */}
      <Pager />
    </Container>
  );
}
