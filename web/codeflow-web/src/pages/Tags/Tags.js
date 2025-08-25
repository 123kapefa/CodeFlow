import React, { useEffect, useState } from "react";
import {
  Container,
  Row,
  Col,
  Form,
  Pagination,
  ButtonGroup,
  ToggleButton,
} from "react-bootstrap";
import Cookies from "js-cookie";
import { useNavigate } from "react-router-dom";
import TagCard from "../../components/TagCard/TagCard";
import { jwtDecode } from "jwt-decode";
import { useAuthFetch } from "../../features/useAuthFetch/useAuthFetch";
import { useAuth } from "../../features/Auth/AuthProvider ";
import "./Tags.css"

import { API_BASE } from "../../config";

function Tags() {
  const [tags, setTags] = useState([]);
  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [orderBy, setOrderBy] = useState("Name");
  const [showPopoverId, setShowPopoverId] = useState(null);
  const authFetch = useAuthFetch();
  const { user, loading } = useAuth();

  const [watchedTags, setWatchedTags] = useState({}); // tagId -> true/false

  const navigate = useNavigate();
  const token = Cookies.get("jwt");


  useEffect(() => {
    const fetchTags = async () => {
      try {
        let orderField = "Name";
        let sortDirection = 0;

        if (orderBy === "New") {
          orderField = "CreatedAt";
          sortDirection = 1;
        }

        const response = await fetch(
          `${API_BASE}/tags?Page=${page}&PageSize=36&OrderBy=${orderField}&SortDirection=${sortDirection}&SearchValue=${search}`
        );

        if (!response.ok) throw new Error("Ошибка загрузки тегов");

        const data = await response.json();

        setTags(data.value);
        setTotalPages(data.pagedInfo.totalPages);
      } catch (error) {
        console.error(error);
      }
    };

    fetchTags();
  }, [page, search, orderBy]);

  const handlePopoverShow = (tagId) => {
    setShowPopoverId(tagId);
  };

  const handlePopoverHide = () => {
    setShowPopoverId(null);
  };

  return (
    <Container className="my-4">
      <h2>Tags</h2>
      <p>
        A tag is a keyword or label that categorizes your question with other,
        similar questions.
      </p>

      <div className="justify-content-sm-between tags-toolbar d-flex align-items-center gap-2 mb-4">
        <Form className="me-2" style={{ maxWidth: "300px" }}>
          <Form.Control
            type="text"
            placeholder="Filter by tag name"
            value={search}
            onChange={(e) => {
              setSearch(e.target.value);
              setPage(1);
            }}
          />
        </Form>

        <ButtonGroup className="flex-shrink-0" style={{ whiteSpace: "nowrap" }}>
          <ToggleButton
            id="name"
            type="radio"
            variant={orderBy === "Name" ? "primary" : "outline-secondary"}
            checked={orderBy === "Name"}
            onChange={() => setOrderBy("Name")}
          >
            Name
          </ToggleButton>
          <ToggleButton
            id="new"
            type="radio"
            variant={orderBy === "New" ? "primary" : "outline-secondary"}
            checked={orderBy === "New"}
            onChange={() => setOrderBy("New")}
          >
            New
          </ToggleButton>
        </ButtonGroup>
      </div>

      <Row>
        {tags.map((tag) => (
          <Col key={tag.id} xs={12} sm={6} md={4} lg={3} className="mb-4">
            <TagCard
              tag={tag}
              isWatched={watchedTags[String(tag.id)]}
              showPopoverId={showPopoverId}
              onPopoverShow={handlePopoverShow}
              onPopoverHide={handlePopoverHide}
            />
          </Col>
        ))}
      </Row>

      <div className="d-flex justify-content-center">
        <Pagination>
          <Pagination.First disabled={page === 1} onClick={() => setPage(1)} />
          <Pagination.Prev
            disabled={page === 1}
            onClick={() => setPage((prev) => Math.max(prev - 1, 1))}
          />
          {[...Array(totalPages)].map((_, index) => (
            <Pagination.Item
              key={index + 1}
              active={index + 1 === page}
              onClick={() => setPage(index + 1)}
            >
              {index + 1}
            </Pagination.Item>
          ))}
          <Pagination.Next
            disabled={page === totalPages}
            onClick={() => setPage((prev) => Math.min(prev + 1, totalPages))}
          />
          <Pagination.Last
            disabled={page === totalPages}
            onClick={() => setPage(totalPages)}
          />
        </Pagination>
      </div>
    </Container>
  );
}

export default Tags;
