// import React, { useState, useEffect } from "react";
// import {
//   Container,
//   Form,
//   Button,
//   Card,
//   Row,
//   Col,
//   Spinner,
// } from "react-bootstrap";
// import { useNavigate } from "react-router-dom";
// import ReactQuill from "react-quill";
// import { toast } from "react-toastify";

// import { useAuth } from "../../features/Auth/AuthProvider ";
// import { useAuthFetch } from "../../features/useAuthFetch/useAuthFetch";

// const tagRegex = /^[a-z0-9.+-]+$/;

// const API = "http://localhost:5000";

// const modules = {
//   toolbar: [
//     [{ header: [1, 2, false] }],
//     ["bold", "italic", "underline", "code-block"],
//     [{ list: "ordered" }, { list: "bullet" }],
//     ["link"],
//     ["clean"],
//   ],
// };

// const formats = [
//   "header",
//   "bold",
//   "italic",
//   "underline",
//   "code-block",
//   "list",
//   "bullet",
//   "link",
// ];

// function CreateQuestion() {
//   const navigate = useNavigate();

//   const { user, loading } = useAuth(); // 1. Хуки первыми
//   const fetchAuth = useAuthFetch();

//   const [title, setTitle] = useState("");
//   const [content, setContent] = useState("");
//   const [tags, setTags] = useState("");
//   const [posting, setPosting] = useState(false);

//   const [tagInput, setTagInput] = useState("");
//   const [suggestions, setSuggestions] = useState([]);
//   const [selectedTags, setSelectedTags] = useState([]);

//   useEffect(() => {
//     if (tagInput.length < 2) {
//       setSuggestions([]);
//       return;
//     }

//     const timeout = setTimeout(async () => {
//       try {
//         const res = await fetch(
//           `${API}/api/tags?Page=1&PageSize=8&OrderBy=Name&SortDirection=1&SearchValue=${tagInput}`
//         );

//         if (res.ok) {
//           const data = await res.json();
//           const items = data.value ?? [];
//           setSuggestions(
//             items.map((t) => ({
//               id: t.id,
//               name: t.name,
//               description: t.description,
//             }))
//           );
//         } else {
//           console.error("Ошибка при получении тегов", res.status);
//         }
//       } catch {
//         setSuggestions([]);
//       }
//     }, 300); // debounce

//     return () => clearTimeout(timeout);
//   }, [tagInput]);

//   // 2. Показываем спиннер во время загрузки профиля
//   if (loading) {
//     return (
//       <Container className="py-5 text-center">
//         <Spinner animation="border" />
//       </Container>
//     );
//   }

//   // 3. Если не залогинен, редирект
//   if (!user) {
//     navigate("/login");
//     return null;
//   }

//   // 4. Отправка формы
//   const handleSubmit = async (e) => {
//     e.preventDefault();

//     const errors = [];

//     if (!title.trim()) {
//       errors.push("Title is required");
//     }

//     if (!content || content.replace(/<(.|\n)*?>/g, "").trim().length < 20) {
//       errors.push("Problem details are required (min 20 characters)");
//     }

//     if (selectedTags.length === 0) {
//       errors.push("At least one tag is required");
//     }

//     for (const tag of selectedTags) {
//       if (!tagRegex.test(tag.name)) {
//         errors.push(
//           `Tag "${tag.name}" is invalid. Only lowercase letters, numbers, "+", "-", "." allowed`
//         );
//       }
//       if (tag.name.length > 64) {
//         errors.push(`Tag "${tag.name}" is too long (max 64 characters)`);
//       }
//     }

//     if (errors.length > 0) {
//       errors.forEach((err) => toast.error(err));
//       return;
//     }

//     const tagList = selectedTags.map((tag) => ({
//       id: tag.id ?? null,
//       name: tag.name,
//       description: tag.description ?? null,
//     }));

//     const payload = {
//       questionDto: {
//         userId: user.userId,
//         title,
//         content,
//         newTags: tagList,
//       },
//     };

//     try {
//       setPosting(true);
//       const res = await fetchAuth(`${API}/api/aggregate/create-question`, {
//         method: "POST",
//         body: JSON.stringify(payload),
//       });

//       if (!res.ok) {
//         const err = await res.json();
//         console.error(err);
//         throw new Error("Не удалось отправить вопрос.");
//       }

//       const result = await res.json();

//       toast.success("Вопрос создан 🎉", {
//         onClose: () => navigate("/questions"),
//         autoClose: 1000,
//       });

//     } catch (err) {
//       alert(err.message);
//     } finally {
//       setPosting(false);
//     }
//   };

//   const handleTagKeyDown = (e) => {
//   if (e.key === "Enter" || e.key === " ") {
//     e.preventDefault();
//     const trimmed = tagInput.trim().toLowerCase();
//     if (!trimmed) return;

//     if (!tagRegex.test(trimmed)) {
//       toast.error(
//         `Invalid tag "${trimmed}". Only lowercase letters, numbers, "+", "-", "." allowed`
//       );
//       return;
//     }

//     if (trimmed.length > 64) {
//       toast.error(`Tag "${trimmed}" is too long (max 64 characters)`);
//       return;
//     }

//     const alreadyExists = selectedTags.some(
//       (tag) => tag.name.toLowerCase() === trimmed
//     );
//     if (alreadyExists || selectedTags.length >= 5) return;

//     const match = suggestions.find(
//       (tag) => tag.name.toLowerCase() === trimmed
//     );

//     const newTag = match || { name: trimmed };
//     setSelectedTags([...selectedTags, newTag]);
//     setTagInput("");
//     setSuggestions([]);
//   }
// };

//   // 5. Основная форма
//   return (
//     <Container className="mt-4">
//       <Card>
//         <Card.Body>
//           <Card.Title className="mb-5">
//             <h3>Ask a question</h3>
//             <p className="text-muted">
//               A private space to help new users write their first questions.
//             </p>
//           </Card.Title>

//           <Form onSubmit={handleSubmit}>
//             <Form.Group controlId="title" className="mb-5">
//               <Form.Label className="text-start d-block">
//                 <strong>Title</strong>
//               </Form.Label>
//               <Form.Text className="text-muted d-block mb-1 text-start">
//                 Be specific and imagine you’re asking a question to another
//                 person.
//               </Form.Text>
//               <Form.Control
//                 type="text"
//                 placeholder="e.g. Is there an R function for finding the index of an element in a vector?"
//                 value={title}
//                 onChange={(e) => setTitle(e.target.value)}
//                 required
//               />
//             </Form.Group>

//             <Form.Group controlId="content" className="mb-5 text-start">
//               <Form.Label className="text-start d-block">
//                 <strong>What are the details of your problem?</strong>
//               </Form.Label>
//               <Form.Text className="text-muted d-block mb-1 text-start">
//                 Introduce the problem and expand on what you put in the title.
//                 Minimum 20 characters.
//               </Form.Text>
//               <ReactQuill
//                 theme="snow"
//                 value={content}
//                 onChange={setContent}
//                 modules={modules}
//                 formats={formats}
//                 style={{ marginBottom: "40px" }}
//               />
//             </Form.Group>

//             <Form.Group controlId="tags" className="mb-4">
//               <Form.Label className="text-start d-block">
//                 <strong>Tags</strong>
//               </Form.Label>
//               <Form.Text className="text-muted d-block mb-1 text-start">
//                 Add up to 5 tags to describe what your question is about.
//               </Form.Text>

//               <Form.Control
//                 type="text"
//                 placeholder="Start typing to see suggestions"
//                 value={tagInput}
//                 onChange={(e) => setTagInput(e.target.value)}
//                 onKeyDown={handleTagKeyDown}
//                 disabled={selectedTags.length >= 5}
//               />

//               {/* Список предложений */}
//               {suggestions.length > 0 && (
//                 <ul className="list-group mt-1 position-absolute z-1 w-50">
//                   {suggestions.map((tag) => (
//                     <li
//                       key={tag.id ?? tag.name}
//                       className="list-group-item list-group-item-action"
//                       onClick={() => {
//                         if (selectedTags.find((t) => t.name === tag.name))
//                           return;
//                         if (selectedTags.length >= 5) return;
//                         setSelectedTags([...selectedTags, tag]);
//                         setTagInput("");
//                         setSuggestions([]);
//                       }}
//                       style={{ cursor: "pointer" }}
//                     >
//                       {tag.name}
//                     </li>
//                   ))}
//                 </ul>
//               )}

//               {/* Отображение выбранных тегов */}
//               <div className="mt-2 d-flex flex-wrap gap-2">
//                 {selectedTags.map((tag, index) => (
//                   <span key={index} className="badge bg-primary">
//                     {tag.name}{" "}
//                     <span
//                       style={{ cursor: "pointer", marginLeft: 6 }}
//                       onClick={() =>
//                         setSelectedTags(
//                           selectedTags.filter((_, i) => i !== index)
//                         )
//                       }
//                     >
//                       &times;
//                     </span>
//                   </span>
//                 ))}
//               </div>

//               {/* Подсказка об ограничении */}
//               {selectedTags.length >= 5 && (
//                 <Form.Text className="text-danger mt-1 d-block">
//                   You can only add up to 5 tags.
//                 </Form.Text>
//               )}
//             </Form.Group>

//             <Row>
//               <Col>
//                 <Button
//                   className="me-auto d-block mb-3"
//                   type="submit"
//                   disabled={posting}
//                 >
//                   {posting ? "Posting..." : "Review your question"}
//                 </Button>
//               </Col>
//             </Row>
//           </Form>
//         </Card.Body>
//       </Card>
//     </Container>
//   );
// }

// export default CreateQuestion;

import React, { useEffect, useState } from "react";
import {
  Container, Form, Button, Card, Row, Col, Spinner,
} from "react-bootstrap";
import { useNavigate, useParams } from "react-router-dom";
import ReactQuill from "react-quill";
import { toast } from "react-toastify";

import { useAuth } from "../../features/Auth/AuthProvider ";
import { useAuthFetch } from "../../features/useAuthFetch/useAuthFetch";

const API = "http://localhost:5000";
const tagRegex = /^[a-z0-9.+-]+$/;

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
  "header","bold","italic","underline","code-block","list","bullet","link",
];

export default function CreateOrEditQuestion() {
  const navigate = useNavigate();
  const { id } = useParams();               // если есть — режим редактирования
  const isEdit = !!id;

  const { user, loading } = useAuth();
  const fetchAuth = useAuthFetch();

  const [title, setTitle] = useState("");
  const [content, setContent] = useState("");
  const [posting, setPosting] = useState(false);

  const [tagInput, setTagInput] = useState("");
  const [suggestions, setSuggestions] = useState([]);
  const [selectedTags, setSelectedTags] = useState([]); // [{id, name, description?}]

  // 1) Редирект если не залогинен
  useEffect(() => {
    if (!loading && !user) navigate("/login");
  }, [loading, user, navigate]);

  // 2) Подсказки тегов (как у тебя)
  useEffect(() => {
    if (tagInput.length < 2) { setSuggestions([]); return; }
    const t = setTimeout(async () => {
      try {
        const res = await fetch(
          `${API}/api/tags?Page=1&PageSize=8&OrderBy=Name&SortDirection=1&SearchValue=${tagInput}`
        );
        if (!res.ok) return setSuggestions([]);
        const data = await res.json();
        const items = data.value ?? [];
        setSuggestions(
          items.map(t => ({ id: t.id, name: t.name, description: t.description }))
        );
      } catch { setSuggestions([]); }
    }, 300);
    return () => clearTimeout(t);
  }, [tagInput]);

  // 3) Подгрузка данных вопроса при редактировании
  useEffect(() => {
    if (!isEdit) return;
    (async () => {
      try {
        const r = await fetch(`${API}/api/aggregate/get-question`, {
          method: "POST",
          headers: { "Content-Type": "application/json", Accept: "application/json" },
          body: JSON.stringify({ questionId: id }),
        });
        if (!r.ok) throw new Error(`HTTP ${r.status}`);
        const data = await r.json();

        // безопасность: только автор 
        if (user && (user.userId !== data.question.userId)) {
          toast.error("Вы не являетесь автором этого вопроса");
          navigate(`/questions/${id}`);
          return;
        }

        setTitle(data.question.title);       // Title readonly
        setContent(data.question.content);

        const tagsFromServer =
          (data.question.questionTags || []).map(qt => ({
            id: qt.tagId,
            name: data.tags[`tag-${qt.tagId}`]?.name || "",
            description: data.tags[`tag-${qt.tagId}`]?.description || "",
          }));
        setSelectedTags(tagsFromServer);
      } catch (e) {
        console.error(e);
        toast.error("Не удалось загрузить вопрос");
        navigate(`/questions/${id}`);
      }
    })();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [isEdit, id, user]);

  if (loading || (isEdit && !title && !content && !selectedTags.length)) {
    // простая защита: показать спиннер, пока редактирование не префиллится
    return (
      <Container className="py-5 text-center">
        <Spinner animation="border" />
      </Container>
    );
  }

  // 4) Сабмит (создание ИЛИ обновление)
  const handleSubmit = async (e) => {
    e.preventDefault();

    const errors = [];

    if (!isEdit && !title.trim()) errors.push("Title is required");
    const plain = content.replace(/<(.|\n)*?>/g, "").trim();
    if (!plain || plain.length < 20) {
      errors.push("Problem details are required (min 20 characters)");
    }
    if (selectedTags.length === 0) errors.push("At least one tag is required");

    for (const tag of selectedTags) {
      if (!tagRegex.test(tag.name)) {
        errors.push(`Tag "${tag.name}" is invalid. Only lowercase letters, numbers, "+", "-", "." allowed`);
      }
      if (tag.name.length > 64) {
        errors.push(`Tag "${tag.name}" is too long (max 64 characters)`);
      }
    }

    if (errors.length) {
      errors.forEach(toast.error);
      return;
    }

    try {
      setPosting(true);

      if (isEdit) {
        // ---- ОБНОВЛЕНИЕ ВОПРОСА ----
        const dto = {
          id,                                  // Guid вопроса
          userEditorId: user.userId,           // кто редактирует
          content,                             // HTML из ReactQuill
          questionTagsDTO: selectedTags.map(t => ({ tagId: t.id ?? t.tagId })),          
        };

        const res = await fetchAuth(`${API}/api/questions`, {
          method: "PUT",
          headers: { "Content-Type": "application/json", Accept: "application/json" },
          body: JSON.stringify(dto),
        });
        if (!res.ok) throw new Error(await res.text() || "Не удалось обновить вопрос");

        toast.success("Изменения сохранены 🎉", {
          onClose: () => navigate(`/questions/${id}`),
          autoClose: 1000,
        });
      } else {
        // ---- СОЗДАНИЕ ВОПРОСА ----
        const tagList = selectedTags.map(tag => ({
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

        const res = await fetchAuth(`${API}/api/aggregate/create-question`, {
          method: "POST",
          body: JSON.stringify(payload),
        });
        if (!res.ok) throw new Error(await res.text() || "Не удалось отправить вопрос");

        toast.success("Вопрос создан 🎉", {
          onClose: () => navigate("/"),
          autoClose: 1000,
        });
      }
    } catch (err) {
      toast.error(err.message);
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
        toast.error(`Invalid tag "${trimmed}". Only lowercase letters, numbers, "+", "-", "." allowed`);
        return;
      }
      if (trimmed.length > 64) {
        toast.error(`Tag "${trimmed}" is too long (max 64 characters)`);
        return;
      }
      const already = selectedTags.some(t => t.name.toLowerCase() === trimmed);
      if (already || selectedTags.length >= 5) return;

      const match = suggestions.find(t => t.name.toLowerCase() === trimmed);
      const newTag = match || { name: trimmed };
      setSelectedTags([...selectedTags, newTag]);
      setTagInput("");
      setSuggestions([]);
    }
  };

  return (
    <Container className="mt-4">
      <Card>
        <Card.Body>
          <Card.Title className="mb-5">
            <h3>{isEdit ? "Edit question" : "Ask a question"}</h3>
            <p className="text-muted">
              {isEdit
                ? "Update your question details and tags."
                : "A private space to help new users write their first questions."}
            </p>
          </Card.Title>

          <Form onSubmit={handleSubmit}>
            
            {!isEdit && (
              <Form.Group controlId="title" className="mb-5">
                <Form.Label className="text-start d-block"><strong>Title</strong></Form.Label>
                <Form.Text className="text-muted d-block mb-1 text-start">
                  Be specific and imagine you’re asking a question to another person.
                </Form.Text>
                <Form.Control
                  type="text"
                  placeholder="e.g. Is there an R function for finding the index of an element in a vector?"
                  value={title}
                  onChange={(e) => setTitle(e.target.value)}
                  required
                />
              </Form.Group>
            )}
            {isEdit && (
              <Form.Group className="mb-3">
                <Form.Label className="text-start d-block"><strong>Title</strong></Form.Label>
                <Form.Control value={title} readOnly />
                <Form.Text className="text-muted d-block text-start">
                  Заголовок сейчас нельзя менять (нет в UpdateQuestionDTO).
                </Form.Text>
              </Form.Group>
            )}

            <Form.Group controlId="content" className="mb-5 text-start">
              <Form.Label className="text-start d-block">
                <strong>What are the details of your problem?</strong>
              </Form.Label>
              <Form.Text className="text-muted d-block mb-1 text-start">
                Introduce the problem and expand on what you put in the title. Minimum 20 characters.
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
              <Form.Label className="text-start d-block"><strong>Tags</strong></Form.Label>
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

              {suggestions.length > 0 && (
                <ul className="list-group mt-1 position-absolute z-1 w-50">
                  {suggestions.map((tag) => (
                    <li
                      key={tag.id ?? tag.name}
                      className="list-group-item list-group-item-action"
                      onClick={() => {
                        if (selectedTags.find((t) => t.name === tag.name)) return;
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

              <div className="mt-2 d-flex flex-wrap gap-2">
                {selectedTags.map((tag, index) => (
                  <span key={index} className="badge bg-primary">
                    {tag.name}
                    <span
                      style={{ cursor: "pointer", marginLeft: 6 }}
                      onClick={() =>
                        setSelectedTags(selectedTags.filter((_, i) => i !== index))
                      }
                    >
                      &times;
                    </span>
                  </span>
                ))}
              </div>

              {selectedTags.length >= 5 && (
                <Form.Text className="text-danger mt-1 d-block">
                  You can only add up to 5 tags.
                </Form.Text>
              )}
            </Form.Group>

            <Row>
              <Col>
                <Button className="me-auto d-block mb-3" type="submit" disabled={posting}>
                  {posting
                    ? (isEdit ? "Saving..." : "Posting...")
                    : (isEdit ? "Save edits" : "Review your question")}
                </Button>
              </Col>
            </Row>
          </Form>
        </Card.Body>
      </Card>
    </Container>
  );
}
