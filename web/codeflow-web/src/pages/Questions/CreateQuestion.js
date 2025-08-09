import React, { useState, useEffect } from "react";
import {
  Container,
  Form,
  Button,
  Card,
  Row,
  Col,
  Spinner,
} from "react-bootstrap";
import { useNavigate } from "react-router-dom";
import ReactQuill from "react-quill";
import { toast } from "react-toastify";

import { useAuth } from "../../features/Auth/AuthProvider ";
import { useAuthFetch } from "../../features/useAuthFetch/useAuthFetch";

const tagRegex = /^[a-z0-9.+-]+$/;

const API = "http://localhost:5000";

const modules = {
  toolbar: [
    [{ header: [1, 2, false] }],
    ["bold", "italic", "underline", "code-block"],
    [{ list: "ordered" }, { list: "bullet" }],
    ["link"],
    ["clean"],
  ],
};

const formats = [
  "header",
  "bold",
  "italic",
  "underline",
  "code-block",
  "list",
  "bullet",
  "link",
];

function CreateQuestion() {
  const navigate = useNavigate();

  const { user, loading } = useAuth(); // 1. Хуки первыми
  const fetchAuth = useAuthFetch();

  const [title, setTitle] = useState("");
  const [content, setContent] = useState("");
  const [tags, setTags] = useState("");
  const [posting, setPosting] = useState(false);

  const [tagInput, setTagInput] = useState("");
  const [suggestions, setSuggestions] = useState([]);
  const [selectedTags, setSelectedTags] = useState([]);

  useEffect(() => {
    if (tagInput.length < 2) {
      setSuggestions([]);
      return;
    }

    const timeout = setTimeout(async () => {
      try {
        const res = await fetch(
          `${API}/api/tags?Page=1&PageSize=8&OrderBy=Name&SortDirection=1&SearchValue=${tagInput}`
        );

        if (res.ok) {
          const data = await res.json();
          const items = data.value ?? [];
          setSuggestions(
            items.map((t) => ({
              id: t.id,
              name: t.name,
              description: t.description,
            }))
          );
        } else {
          console.error("Ошибка при получении тегов", res.status);
        }
      } catch {
        setSuggestions([]);
      }
    }, 300); // debounce

    return () => clearTimeout(timeout);
  }, [tagInput]);

  // 2. Показываем спиннер во время загрузки профиля
  if (loading) {
    return (
      <Container className="py-5 text-center">
        <Spinner animation="border" />
      </Container>
    );
  }

  // 3. Если не залогинен, редирект
  if (!user) {
    navigate("/login");
    return null;
  }

  // 4. Отправка формы
  const handleSubmit = async (e) => {
    e.preventDefault();

    const errors = [];

    if (!title.trim()) {
      errors.push("Title is required");
    }

    if (!content || content.replace(/<(.|\n)*?>/g, "").trim().length < 20) {
      errors.push("Problem details are required (min 20 characters)");
    }

    if (selectedTags.length === 0) {
      errors.push("At least one tag is required");
    }

    for (const tag of selectedTags) {
      if (!tagRegex.test(tag.name)) {
        errors.push(
          `Tag "${tag.name}" is invalid. Only lowercase letters, numbers, "+", "-", "." allowed`
        );
      }
      if (tag.name.length > 64) {
        errors.push(`Tag "${tag.name}" is too long (max 64 characters)`);
      }
    }

    if (errors.length > 0) {
      errors.forEach((err) => toast.error(err));
      return;
    }

    const tagList = selectedTags.map((tag) => ({
      id: tag.id ?? null,
      name: tag.name,
      description: tag.description ?? null,
    }));

    const payload = {
      questionDto: {
        userId: user.userId, 
        title,
        content,
        newTags: tagList,
      },
    };

    try {
      setPosting(true);
      const res = await fetchAuth(`${API}/api/aggregate/create-question`, {
        method: "POST",
        body: JSON.stringify(payload),
      });

      if (!res.ok) {
        const err = await res.json();
        console.error(err);
        throw new Error("Не удалось отправить вопрос.");
      }

      const result = await res.json();

      toast.success("Вопрос создан 🎉", {
        onClose: () => navigate("/questions"),
        autoClose: 1000,
      });
      
    } catch (err) {
      alert(err.message);
    } finally {
      setPosting(false);
    }
  };

  const handleTagKeyDown = (e) => {
  if (e.key === "Enter" || e.key === " ") {
    e.preventDefault();
    const trimmed = tagInput.trim().toLowerCase();
    if (!trimmed) return;

    if (!tagRegex.test(trimmed)) {
      toast.error(
        `Invalid tag "${trimmed}". Only lowercase letters, numbers, "+", "-", "." allowed`
      );
      return;
    }

    if (trimmed.length > 64) {
      toast.error(`Tag "${trimmed}" is too long (max 64 characters)`);
      return;
    }

    const alreadyExists = selectedTags.some(
      (tag) => tag.name.toLowerCase() === trimmed
    );
    if (alreadyExists || selectedTags.length >= 5) return;

    const match = suggestions.find(
      (tag) => tag.name.toLowerCase() === trimmed
    );

    const newTag = match || { name: trimmed };
    setSelectedTags([...selectedTags, newTag]);
    setTagInput("");
    setSuggestions([]);
  }
};

  // 5. Основная форма
  return (
    <Container className="mt-4">
      <Card>
        <Card.Body>
          <Card.Title className="mb-5">
            <h3>Ask a question</h3>
            <p className="text-muted">
              A private space to help new users write their first questions.
            </p>
          </Card.Title>

          <Form onSubmit={handleSubmit}>
            <Form.Group controlId="title" className="mb-5">
              <Form.Label className="text-start d-block">
                <strong>Title</strong>
              </Form.Label>
              <Form.Text className="text-muted d-block mb-1 text-start">
                Be specific and imagine you’re asking a question to another
                person.
              </Form.Text>
              <Form.Control
                type="text"
                placeholder="e.g. Is there an R function for finding the index of an element in a vector?"
                value={title}
                onChange={(e) => setTitle(e.target.value)}
                required
              />
            </Form.Group>

            <Form.Group controlId="content" className="mb-5">
              <Form.Label className="text-start d-block">
                <strong>What are the details of your problem?</strong>
              </Form.Label>
              <Form.Text className="text-muted d-block mb-1 text-start">
                Introduce the problem and expand on what you put in the title.
                Minimum 20 characters.
              </Form.Text>
              <ReactQuill
                theme="snow"
                value={content}
                onChange={setContent}
                modules={modules}
                formats={formats}
                style={{ marginBottom: "40px" }}
              />
            </Form.Group>

            <Form.Group controlId="tags" className="mb-4">
              <Form.Label className="text-start d-block">
                <strong>Tags</strong>
              </Form.Label>
              <Form.Text className="text-muted d-block mb-1 text-start">
                Add up to 5 tags to describe what your question is about.
              </Form.Text>

              <Form.Control
                type="text"
                placeholder="Start typing to see suggestions"
                value={tagInput}
                onChange={(e) => setTagInput(e.target.value)}
                onKeyDown={handleTagKeyDown}
                disabled={selectedTags.length >= 5}
              />

              {/* Список предложений */}
              {suggestions.length > 0 && (
                <ul className="list-group mt-1 position-absolute z-1 w-50">
                  {suggestions.map((tag) => (
                    <li
                      key={tag.id ?? tag.name}
                      className="list-group-item list-group-item-action"
                      onClick={() => {
                        if (selectedTags.find((t) => t.name === tag.name))
                          return;
                        if (selectedTags.length >= 5) return;
                        setSelectedTags([...selectedTags, tag]);
                        setTagInput("");
                        setSuggestions([]);
                      }}
                      style={{ cursor: "pointer" }}
                    >
                      {tag.name}
                    </li>
                  ))}
                </ul>
              )}

              {/* Отображение выбранных тегов */}
              <div className="mt-2 d-flex flex-wrap gap-2">
                {selectedTags.map((tag, index) => (
                  <span key={index} className="badge bg-primary">
                    {tag.name}{" "}
                    <span
                      style={{ cursor: "pointer", marginLeft: 6 }}
                      onClick={() =>
                        setSelectedTags(
                          selectedTags.filter((_, i) => i !== index)
                        )
                      }
                    >
                      &times;
                    </span>
                  </span>
                ))}
              </div>

              {/* Подсказка об ограничении */}
              {selectedTags.length >= 5 && (
                <Form.Text className="text-danger mt-1 d-block">
                  You can only add up to 5 tags.
                </Form.Text>
              )}
            </Form.Group>

            <Row>
              <Col>
                <Button
                  className="me-auto d-block mb-3"
                  type="submit"
                  disabled={posting}
                >
                  {posting ? "Posting..." : "Review your question"}
                </Button>
              </Col>
            </Row>
          </Form>
        </Card.Body>
      </Card>
    </Container>
  );
}

export default CreateQuestion;
