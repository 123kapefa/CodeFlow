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

    // пагинация
    q.append("Page", String(page));
    q.append("PageSize", String(PAGE_SIZE));

    // поиск
    if (filter.trim()) q.append("SearchValue", filter.trim());

    // сортировка
    if (sort === "reputation") {
      q.append("OrderBy", "Reputation");
      q.append("SortDirection", "Descending"); // топ репутация сверху
    } else {
      q.append("OrderBy", "CreatedAt");
      q.append("SortDirection", "Descending"); // самые новые сверху
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

        const dto = await res.json(); // { value: User[], pagedInfo: { totalPages, ... } }
        setUsers(dto.value ?? []);
        setPagesTotal(dto.pagedInfo?.totalPages ?? 1);
        setError(null);
      } catch (e) {
        if (e.name !== "AbortError") setError(e.message || "Request failed");
      } finally {
        setLoading(false);
      }
    })();

    return () => ctrl.abort();
  }, [query]);

  /* ----------------------- pagination ---------------------------- */
  const Pager = () => {
    if (pagesTotal <= 1) return null;

    const items = [];
    const window = 5;
    const head = Math.min(pagesTotal, window);

    items.push(
      <Pagination.Prev
        key="prev"
        disabled={page === 1}
        onClick={() => setPage((p) => Math.max(1, p - 1))}
      />
    );

    for (let p = 1; p <= head; p++) {
      items.push(
        <Pagination.Item key={p} active={p === page} onClick={() => setPage(p)}>
          {p}
        </Pagination.Item>
      );
    }

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

      {loading && (
        <div className="text-center my-5">
          <Spinner animation="border" role="status" />
        </div>
      )}

      {error && <Alert variant="danger">Ошибка: {error}</Alert>}

      <Row>
        {users.map((u) => (
          <UserCard key={u.id ?? u.userName} user={u} />
        ))}
      </Row>

      <Pager />
    </Container>
  );
}
