import { useNavigate, useParams, Link } from "react-router-dom";
import React, { useEffect, useState, useCallback } from "react";
import { Container, Spinner, Badge, Card, Button, Form } from "react-bootstrap";
import { ClockHistory } from "react-bootstrap-icons";

import ReactQuill from "react-quill";
import "react-quill/dist/quill.snow.css";

import hljs from "highlight.js";
import "highlight.js/styles/github.css";
import { toast } from "react-toastify";

import dayjs from "dayjs";
import relativeTime from "dayjs/plugin/relativeTime";
import { useAuth } from "../../features/Auth/AuthProvider ";
import { useAuthFetch } from "../../features/useAuthFetch/useAuthFetch";

import AuthorCard from "../../components/AuthorCard/AuthorCard";
import "./QuestionPage.css";

import { API_BASE } from "../../config";

dayjs.extend(relativeTime);

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

const CommentForm = React.memo(function CommentForm({
  commentText,
  setCommentText,
  commentSubmitting,
  onSubmit,
  onCancel,
}) {
  return (
    <div className="d-flex gap-2 align-items-start w-100">
      <Form.Control
        as="textarea"
        rows={3}
        value={commentText}
        onChange={(e) => setCommentText(e.target.value)}
        placeholder="Use comments to ask for more information or suggest improvements"
        style={{ flex: 1, minWidth: 0 }}
      />
      <div className="d-flex flex-column gap-2">
        <Button
          size="sm"
          variant="primary"
          onClick={onSubmit}
          disabled={commentSubmitting}
        >
          {commentSubmitting ? "Posting..." : "Post"}
        </Button>
        <Button
          size="sm"
          variant="outline-secondary"
          onClick={onCancel}
          disabled={commentSubmitting}
        >
          Cancel
        </Button>
      </div>
    </div>
  );
});

export default function QuestionPage() {
  const { id } = useParams();
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);

  const [answerHtml, setAnswerHtml] = useState("");
  const [submitting, setSubmitting] = useState(false);

  const [openCommentFor, setOpenCommentFor] = useState(null);
  const [commentText, setCommentText] = useState("");
  const [commentSubmitting, setCommentSubmitting] = useState(false);

  const [votingQ, setVotingQ] = useState(false);
  const [votingA, setVotingA] = useState({});
  const [acceptingA, setAcceptingA] = useState({});

  const { user } = useAuth();
  const fetchAuth = useAuthFetch();
  const navigate = useNavigate();

  // кто залогинен
  const currentUserId = (user && (user.userId ?? user.id)) || null;

  // ── helpers ────────────────────────────────────────────────────────────────
  const normalizeResponse = (res) => {
    // Справочники
    const usersMap = Object.fromEntries(
      (res.users ?? []).map((u) => [u.userId, u])
    );
    const tagsMap = Object.fromEntries((res.tags ?? []).map((t) => [t.id, t]));

    // Вопрос + автор
    const qAuthor = usersMap[res.question.userId] ?? {};
    const question = {
      ...res.question,
      authorName: qAuthor.username ?? "unknown",
      authorReputation: qAuthor.reputation ?? 0,
      authorAvatarUrl: qAuthor.avatarUrl ?? null,
    };

    // Ответы + авторы
    const answers = (res.answers ?? [])
      .map((a) => {
        const au = usersMap[a.userId] ?? {};
        return {
          ...a,
          authorName: au.username ?? "unknown",
          authorReputation: au.reputation ?? 0,
          authorAvatarUrl: au.avatarUrl ?? null,
          isAccepted: a.isAccepted ?? res.question?.acceptedAnswerId === a.id,
        };
      })
      // сортировка: новые сверху
      .sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt));

    // Комментарии к вопросу: в ответе сервера authorId
    const questionComments = (res.questionComments ?? []).map((c) => {
      const uid = c.authorId ?? c.userId; // на всякий случай, если формат поменяется
      const cu = usersMap[uid] ?? {};
      return {
        ...c,
        authorName: cu.username ?? "user",
        authorReputation: cu.reputation ?? 0,
        authorAvatarUrl: cu.avatarUrl ?? null,
      };
    });

    // Комментарии к ответам: объект { [answerId]: Comment[] } с authorId
    const answerComments = {};
    if (res.answerComments) {
      for (const [answerId, list] of Object.entries(res.answerComments)) {
        answerComments[answerId] = (list ?? []).map((c) => {
          const uid = c.authorId ?? c.userId;
          const cu = usersMap[uid] ?? {};
          return {
            ...c,
            authorName: cu.username ?? "user",
            authorReputation: cu.reputation ?? 0,
            authorAvatarUrl: cu.avatarUrl ?? null,
          };
        });
      }
    }

    return {
      question,
      answers,
      tagsMap,
      questionComments,
      answerComments,
      users: res.users ?? [],
      tags: res.tags ?? [],
    };
  };

  const sendVote = (payload) =>
    fetchAuth(`${API_BASE}/votes/set`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Accept: "application/json",
      },
      body: JSON.stringify(payload),
    });

  // ЗАГРУЗКА ВОПРОСА
  const loadQuestion = useCallback(async () => {
    setLoading(true);
    try {
      const r = await fetchAuth(`${API_BASE}/aggregate/get-question`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Accept: "application/json",
        },
        body: JSON.stringify({ questionId: id }),
      });
      if (!r.ok) throw new Error(`HTTP error ${r.status}`);
      const text = await r.text();
      if (!text) throw new Error("Empty response");
      const raw = JSON.parse(text);
      setData(normalizeResponse(raw));
    } catch (err) {
      console.error("Ошибка при получении вопроса:", err.message);
    } finally {
      setLoading(false);
    }
  }, [id]);

  useEffect(() => {
    loadQuestion();
  }, [loadQuestion]);

  // Подсветка кода
  useEffect(() => {
    if (!data) return;
    document.querySelectorAll("pre.ql-syntax").forEach((block) => {
      hljs.highlightElement(block);
    });
  }, [data]);

  const handleAnswerSubmit = async (e) => {
    e.preventDefault();
    if (submitting) return;

    if (!user) {
      navigate("/login");
      return;
    }

    const plainText = answerHtml
      ?.replace(/<[^>]*>/g, "")
      .replace(/&nbsp;/g, " ")
      .trim();
    if (!plainText) {
      alert("Ответ не может быть пустым.");
      return;
    }

    const { question, tagsMap } = data;

    const tagsPayload =
      (question?.questionTags ?? []).map((t) => ({
        tagId: t.tagId,
        name: tagsMap?.[t.tagId]?.name || "",
      })) || [];

    const payload = {
      questionId: question.id,
      userId: user.userId,
      content: answerHtml,
      tags: tagsPayload,
    };

    try {
      setSubmitting(true);
      const res = await fetchAuth(`${API_BASE}/answers`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Accept: "application/json",
        },
        body: JSON.stringify(payload),
      });
      if (!res.ok) {
        const errText = await res.text();
        throw new Error(errText || `HTTP ${res.status}`);
      }
      setAnswerHtml("");
      await loadQuestion();
    } catch (err) {
      console.error("Create answer failed:", err);
      alert("Не удалось создать ответ: " + err.message);
    } finally {
      setSubmitting(false);
    }
  };

  // КОММЕНТЫ
  const postComment = async ({ type, targetId, content }) => {
   
    const payload = {
      AuthorId: user.id ?? user.userId,
      Content: content,
      Type: type, // "Question" | "Answer" 
      TargetId: targetId, // GUID вопроса или ответа
    };

    const res = await fetchAuth(`${API_BASE}/comments`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Accept: "application/json",
      },
      body: JSON.stringify(payload),
    });

    return res;
  };

  const handleCommentSubmit = async () => {
    if (commentSubmitting) return;
    if (!user) {
      navigate("/login");
      return;
    }

    const trimmed = commentText.replace(/\s+/g, " ").trim();
    if (!trimmed) return;

    try {
      setCommentSubmitting(true);

      if (openCommentFor === "question") {
        const res = await postComment({
          type: "Question",
          targetId: data.question.id,
          content: trimmed,
        });
        if (!res.ok) throw new Error(await res.text());
      } else {
        const res = await postComment({
          type: "Answer",
          targetId: openCommentFor,
          content: trimmed,
        });
        if (!res.ok) {
          const t = await res.text();
          console.error("POST /comments failed", res.status, t);
          throw new Error(t || `HTTP ${res.status}`);
        }
      }

      setCommentText("");
      setOpenCommentFor(null);
      await loadQuestion();
    } catch (e) {
      console.error("Create comment failed:", e);
      alert("Не удалось создать комментарий");
    } finally {
      setCommentSubmitting(false);
    }
  };

  // Обработка ошибок голосования
  const extractProblemMessage = (text) => {
    try {
      const json = JSON.parse(text);
      if (json?.detail) {
        
        let detail = json.detail
          .replace(/^Next error\(s\) occurred:/i, "")
          .trim();
        
        return detail
          .split("\n")
          .map((l) => l.trim())
          .filter(Boolean)
          .map((l) => l.replace(/^\*+/, "").trim())
          .join("\n");
      }
      if (json?.message) return json.message;
      if (json?.title) return json.title;
    } catch {
      /* not JSON */
    }
    return text;
  };

  // единый обработчик ответа голосования
  const handleVoteResponse = async (res, { rollback } = {}) => {
    if (res.ok) return true;

   
    const bodyText = await res.text();
   
    if (rollback) await rollback();

    if (res.status === 401) {
      toast.info("Войдите, чтобы голосовать");
      navigate("/login");
      return false;
    }

    if (res.status === 409 || res.status === 429) {
      const msg = extractProblemMessage(bodyText); 
      const retryAfter = res.headers.get("Retry-After");
      if (retryAfter) {
        const sec = parseInt(retryAfter, 10);
        const mins = Math.ceil(sec / 60);
        toast.warn(
          `${msg}${
            Number.isFinite(mins) ? ` Повторите через ~${mins} мин.` : ""
          }`
        );
      } else {
        toast.warn(msg || "Конфликт голосования. Попробуйте позже.");
      }
      return false;
    }

    // прочее
    toast.error(
      extractProblemMessage(bodyText) || "Не удалось выполнить голосование"
    );
    return false;
  };

  // ГОЛОСОВАНИЯ
  const voteQuestion = async (value) => {
    if (!user) {
      navigate("/login");
      return;
    }
    if (votingQ) return;

    // запрет голосовать за себя
    if (currentUserId && currentUserId === data.question.userId) {
      toast.warn("Запрещено голосовать за себя!");
      return;
    }

    try {
      setVotingQ(true);

      setData((prev) => ({
        ...prev,
        question: {
          ...prev.question,
          upvotes: prev.question.upvotes + (value === 1 ? 1 : 0),
          downvotes: prev.question.downvotes + (value === -1 ? 1 : 0),
        },
      }));

      const payload = {
        ParentId: data.question.id, // id вопроса
        SourceId: data.question.id, // голосуем за сам вопрос
        OwnerUserId: data.question.userId, // владелец источника
        SourceType: "Question",
        ValueKind: value === 1 ? "Up" : "Down",
      };

      const res = await sendVote(payload);
      const ok = await handleVoteResponse(res, { rollback: loadQuestion });
      if (!ok) return;
    } catch (e) {
      console.error("Question vote failed:", e);
      alert("Не удалось проголосовать за вопрос");
    } finally {
      setVotingQ(false);
    }
  };

  // ГОЛОСОВАНИЯ
  const voteAnswer = async (answerId, value) => {
    if (!user) {
      navigate("/login");
      return;
    }
    if (votingA[answerId]) return;

    const a = data?.answers?.find((x) => x.id === answerId);
    if (a && currentUserId && currentUserId === a.userId) {
      toast.warn("Запрещено голосовать за себя!");
      return;
    }

    try {
      setVotingA((m) => ({ ...m, [answerId]: true }));

      setData((prev) => ({
        ...prev,
        answers: prev.answers.map((a) =>
          a.id === answerId
            ? {
                ...a,
                upvotes: a.upvotes + (value === 1 ? 1 : 0),
                downvotes: a.downvotes + (value === -1 ? 1 : 0),
              }
            : a
        ),
      }));

      const payload = {
        ParentId: data.question.id, // вопрос-владелец
        SourceId: answerId, // источник голосования — ответ
        OwnerUserId: a.userId, // владелец ответа
        SourceType: "Answer",
        ValueKind: value === 1 ? "Up" : "Down",
      };

      const res = await sendVote(payload);
      const ok = await handleVoteResponse(res, { rollback: loadQuestion });
      if (!ok) return;
    } catch (e) {
      console.error("Answer vote failed:", e);
      alert("Не удалось проголосовать за ответ");
    } finally {
      setVotingA((m) => ({ ...m, [answerId]: false }));
    }
  };

  // ПРИНЯТИЕ ОТВЕТА
  const acceptAnswer = async (answerId) => {
    if (!user) {
      navigate("/login");
      return;
    }

    const { question } = data;
    if ((user.id ?? user.userId) !== question.userId) {
      alert("Только автор вопроса может принять ответ.");
      return;
    }

    if (acceptingA[answerId]) return;

    try {
      setAcceptingA((m) => ({ ...m, [answerId]: true }));    

      const oldAnsweredUserId = data.answers.find(a => a.isAccepted === true)?.userId ?? null;
      const userAnswerId = data.answers.find(a => a.id === answerId)?.userId ?? null;    

      const res = await fetchAuth(
        `${API_BASE}/questions/${question.id}/answer-accept`,
        {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
            Accept: "application/json",
          },
          body: JSON.stringify({
            OldAcceptedAnswerId: oldAnsweredUserId,
            AcceptAnswerId: answerId,
            UserAnswerId: userAnswerId,
          }),
        }
      );

      if (!res.ok) {
        const errText = await res.text();
        throw new Error(errText || `HTTP ${res.status}`);
      }

      // локально отметим
      setData((prev) => ({
        ...prev,
        question: {
          ...prev.question,
          acceptedAnswerId: answerId,
          isClosed: true,
        },
        answers: prev.answers.map((a) =>
          a.id === answerId
            ? { ...a, isAccepted: true }
            : { ...a, isAccepted: false }
        ),
      }));

      await loadQuestion();
    } catch (e) {
      console.error("Accept answer failed:", e);
      alert("Не удалось принять ответ");
    } finally {
      setAcceptingA((m) => ({ ...m, [answerId]: false }));
    }
  };

  if (loading) {
    return (
      <div className="text-center my-5">
        <Spinner animation="border" />
      </div>
    );
  }
  if (!data) {
    return <Container className="my-5">Question not found</Container>;
  }

  const { question, answers, tagsMap } = data;
  const createdAtText = `Asked ${dayjs(question.createdAt).fromNow()}`;
  const updatedAtText = question.updatedAt
    ? `Modified ${dayjs(question.updatedAt).fromNow()}`
    : null;
  const viewsText = `Viewed ${question.viewsCount.toLocaleString()} times`;

  const questionComments = data.questionComments ?? [];
  const getAnswerComments = (answerId) =>
    (data.answerComments && data.answerComments[answerId]) ?? [];

  return (
    <Container className="my-4 question-page">
      {/* Заголовок и мета */}
      <header className="mb-3 text-start">
        <div className="d-flex justify-content-between align-items-center">
          <h1 className="qp-title mb-0">{question.title}</h1>
          <Button as={Link} to="/questions/ask" variant="outline-primary">
            Ask Question
          </Button>
        </div>
        <div className="qp-meta">
          <span>{createdAtText}</span>
          {updatedAtText && <span> · {updatedAtText}</span>}
          <span> · {viewsText}</span>
        </div>
      </header>

      {/* Пост-вопрос */}
      <article className="post">
        <div className="vote-col ">
          <Button
            variant="light"
            size="sm"
            className="vote-btn"
            onClick={() => voteQuestion(1)}
            disabled={votingQ}
            title={user ? "Upvote" : "Sign in to vote"}
          >
            ▲
          </Button>
          <div className="vote-score">
            {question.upvotes - question.downvotes}
          </div>
          <Button
            variant="light"
            size="sm"
            className="vote-btn"
            onClick={() => voteQuestion(-1)}
            disabled={votingQ}
            title={user ? "Downvote" : "Sign in to vote"}
          >
            ▼
          </Button>

          <Button
            variant="link"
            size="sm"
            className="p-0 mt-1 history-btn"
            onClick={() => navigate(`/questions/${question.id}/history`)}
            title="View edit history"
            aria-label="View edit history"
          >
            <ClockHistory size={16} />
          </Button>
        </div>

        <div className="post-body text-start">
          <Card className="border-0">
            <Card.Body className="p-0">
              <div
                className="post-content"
                dangerouslySetInnerHTML={{ __html: question.content }}
              />
              <div className="post-tags">
                {(question.questionTags ?? []).map((t) => {
                  const tag = tagsMap?.[t.tagId];
                  const tagName = tag?.name ?? "unknown";
                  return (
                    <Badge
                      key={t.tagId}
                      bg="light"
                      text="dark"
                      className="tag-badge mt-5"
                      title={`tag: ${tagName}`}
                      style={{ cursor: "pointer" }}
                      onClick={() => navigate(`/tags/${t.tagId}/questions`)}
                    >
                      {tagName}
                    </Badge>
                  );
                })}
              </div>
            </Card.Body>
          </Card>
        </div>
      </article>

      {/* Нижний блок под вопросом */}
      <div className="post-footer">
        <div className="post-actions">
          <Button
            variant="link"
            className="p-0 edit-link"
            onClick={() => navigate(`/questions/edit/${question.id}`)}
          >
            Edit
          </Button>
        </div>

        <AuthorCard
          kind="asked"
          dt={question.createdAt}
          userId={question.userId}
          name={question.authorName}
          reputation={question.authorReputation}
          avatarUrl={question.authorAvatarUrl}
        />
      </div>

      {/* Комментарии к вопросу */}
      <div className="mt-3 text-center comment-block">
        {questionComments.length > 0 && (
          <ul className="list-unstyled mb-2">
            <ul className="list-unstyled mb-2">
              {questionComments.map((c) => (
                <li key={c.id} className="mb-1">
                  <span>{c.content}</span>
                  <span className="text-muted small ms-2">
                    –{" "}
                    <Link
                      className="text-decoration-none"
                      to={`/users/${c.authorId}`}
                    >
                      {c.authorName}
                    </Link>{" "}
                    {dayjs(c.createdAt).fromNow()}
                  </span>
                </li>
              ))}
            </ul>
          </ul>
        )}

        <div className="post-actions w-100">
          {openCommentFor === "question" ? (
            <CommentForm
              key="comment-question"
              commentText={commentText}
              setCommentText={setCommentText}
              commentSubmitting={commentSubmitting}
              onSubmit={handleCommentSubmit}
              onCancel={() => setOpenCommentFor(null)}
            />
          ) : (
            <Button
              variant="link"
              className="p-0 edit-link"
              onClick={() => setOpenCommentFor("question")}
            >
              Add comment
            </Button>
          )}
        </div>
      </div>

      {/* Ответы */}
      <h2 className="answers-title text-start pt-5">
        {answers.length} Answers
      </h2>
      {answers.map((a) => (
        <React.Fragment key={a.id}>
          <article className="post">
            <div className="vote-col">
              <Button
                variant="light"
                size="sm"
                className="vote-btn"
                onClick={() => voteAnswer(a.id, 1)}
                disabled={!!votingA[a.id]}
                title={user ? "Upvote" : "Sign in to vote"}
              >
                ▲
              </Button>
              <div className="vote-score">{a.upvotes - a.downvotes}</div>
              <Button
                variant="light"
                size="sm"
                className="vote-btn"
                onClick={() => voteAnswer(a.id, -1)}
                disabled={!!votingA[a.id]}
                title={user ? "Downvote" : "Sign in to vote"}
              >
                ▼
              </Button>

              <div className="text-center mt-1">
                {a.isAccepted ? (
                  <span
                    style={{ color: "green", fontSize: "20px" }}
                    title="Принятый ответ"
                  >
                    ✔
                  </span>
                ) : (
                  user?.userId === question.userId && (
                    <Button
                      variant="outline-success"
                      size="sm"
                      className="vote-btn"
                      onClick={() => acceptAnswer(a.id)}
                      disabled={!!acceptingA[a.id]}
                      title="Отметить как принятый"
                    >
                      ✓
                    </Button>
                  )
                )}
              </div>

              <Button
                variant="link"
                size="sm"
                className="p-0 mt-1 history-btn"
                onClick={() => navigate(`/answers/${a.id}/history`)}
                title="View edit history"
                aria-label="View edit history"
              >
                <ClockHistory size={16} />
              </Button>
            </div>

            <div className="post-body text-start">
              <Card className="border-0">
                <Card.Body className="p-0">
                  <div
                    className="post-content"
                    dangerouslySetInnerHTML={{ __html: a.content }}
                  />
                </Card.Body>
              </Card>
            </div>
          </article>

          <div className="post-footer">
            <div className="post-actions">
              <Button
                variant="link"
                className="p-0 edit-link"
                onClick={() =>
                  navigate(`/answers/edit/${a.id}`, {
                    state: {
                      content: a.content,
                      questionId: question.id,
                      authorId: a.userId,
                    },
                  })
                }
              >
                Edit
              </Button>
            </div>
            <AuthorCard
              kind="answered"
              dt={a.createdAt}
              userId={a.userId}
              name={a.authorName}
              reputation={a.authorReputation}
              avatarUrl={a.authorAvatarUrl}
            />
          </div>

          {/* Комментарии к ответу */}
          <div className="mt-2 text-center">
            {getAnswerComments(a.id).length > 0 && (
              <ul className="list-unstyled mb-2">
                {getAnswerComments(a.id).map((c) => (
                  <li key={c.id} className="mb-1">
                    <span>{c.content}</span>
                    <span className="text-muted small ms-2">
                      –{" "}
                      <Link
                        className="text-decoration-none"
                        to={`/users/${c.authorId}`}
                      >
                        {c.authorName}
                      </Link>{" "}
                      {dayjs(c.createdAt).fromNow()}
                    </span>
                  </li>
                ))}
              </ul>
            )}

            <div className="post-actions">
              {openCommentFor === a.id ? (
                <CommentForm
                  key={`comment-answer-${a.id}`}
                  commentText={commentText}
                  setCommentText={setCommentText}
                  commentSubmitting={commentSubmitting}
                  onSubmit={handleCommentSubmit}
                  onCancel={() => setOpenCommentFor(null)}
                />
              ) : (
                <Button
                  variant="link"
                  className="p-0 edit-link pb-5"
                  onClick={() => setOpenCommentFor(a.id)}
                >
                  Add comment
                </Button>
              )}
            </div>
          </div>
        </React.Fragment>
      ))}

      {/* Форма ответа */}
      <h2 className="your-answer-title text-start pt-3">Your Answer</h2>
      <Form onSubmit={handleAnswerSubmit}>
        <Form.Group controlId="answerHtml" className="mb-3 text-start">
          <ReactQuill
            theme="snow"
            value={answerHtml}
            onChange={setAnswerHtml}
            modules={modules}
            formats={formats}
            style={{ marginBottom: 16 }}
          />
        </Form.Group>
        <Button type="submit" variant="primary" disabled={submitting}>
          {submitting ? "Posting..." : "Post Your Answer"}
        </Button>
      </Form>
    </Container>
  );
}
