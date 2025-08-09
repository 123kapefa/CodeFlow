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
import { useNavigate, Link } from "react-router-dom";
import { useParams, useSearchParams } from "react-router-dom";

import QuestionCard from "../../components/QuestionCard/QuestionCard";

function Questions() {
  /* ───────── URL-параметры ───────── */
  const { tagId } = useParams(); // undefined или id тега
  const [qs, setQs] = useSearchParams();

  const page = parseInt(qs.get("page") ?? "1", 10);
  const orderBy = qs.get("orderBy") ?? "AnswersCount";
  const sortDir = parseInt(qs.get("sortDir") ?? "0", 10); // 0 = Descending

  /* ───────── состояние ───────── */
  const [items, setItems] = useState([]);
  const [pageInfo, setInfo] = useState(null);
  const [loading, setLoading] = useState(true);
  const [currentTag, setCurrentTag] = useState(null);

  /* ───────── загрузка ───────── */
  useEffect(() => {
    setLoading(true);

    const fetchData = async () => {
      setLoading(true);
      const url =
        `http://localhost:5000/api/aggregate/get-questions?page=${page}` +
        `&pageSize=30&orderBy=${orderBy}&sortDirection=${
          sortDir === 1 ? "Ascending" : "Descending"
        }` +
        (tagId ? `&tagId=${tagId}` : "");

      fetch(url, {
        method: "POST",
        headers: {
          Accept: "application/json",
        },
      })
        .then(async (r) => {
          if (!r.ok) throw new Error(`HTTP error ${r.status}`);
          const text = await r.text();
          if (!text) throw new Error("Empty response");
          return JSON.parse(text);
        })
        .then((res) => {
          const tagMap = res.tags;
          const mappedItems = res.questions.value.map((q) => ({
            id: q.id,
            title: q.title,
            votes: q.upvotes - q.downvotes,
            answers: q.answersCount,
            views: q.viewsCount,
            tags: q.questionTags.map(
              (t) => tagMap[`tag-${t.tagId}`]?.name ?? "unknown"
            ),
            author: "unknown",
            answeredAgo: new Date(q.createdAt).toLocaleDateString(),
          }));
          setItems(mappedItems);
          setInfo(res.questions.pagedInfo);

          if (tagId && tagMap[`tag-${tagId}`]) {
            setCurrentTag(tagMap[`tag-${tagId}`].name);
          } else {
            setCurrentTag(null);
          }
        })
        .catch((err) => {
          console.error("Ошибка при получении вопросов:", err.message);
        })
        .finally(() => setLoading(false));
    };

    fetchData();
  }, [page, orderBy, sortDir, tagId]);

  const setPageQuery = (p) => {
    qs.set("page", p);
    setQs(qs);
  };

  const setSort = (field, dir) => {
    qs.set("orderBy", field);
    qs.set("sortDir", dir);
    qs.set("page", 1);
    setQs(qs);
  };

  return (
    <Container className="my-4">
      <Row className="align-items-center mb-5">
        <Col>
          <h2 className="mb-3">
            {tagId ? (
              <>Questions tagged [{currentTag ?? tagId}]</>
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
            onChange={() => setSort("AnswersCount", 0)} // descending
          >
            Answered
          </ToggleButton>

          <ToggleButton
            id="sort-new"
            type="radio"
            variant={orderBy === "CreatedAt" ? "primary" : "outline-secondary"}
            checked={orderBy === "CreatedAt"}
            onChange={() => setSort("CreatedAt", 1)}
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

          {pageInfo && (
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
