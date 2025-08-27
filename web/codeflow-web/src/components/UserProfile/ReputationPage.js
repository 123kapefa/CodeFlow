import React, { useEffect, useState, useCallback } from "react";
import {
  Button,
  ButtonGroup,
  Pagination,
  Spinner,
  ToggleButton,
} from "react-bootstrap";
import PropTypes from "prop-types";
import { CaretRightFill, CaretDownFill } from "react-bootstrap-icons";

import { useAuthFetch } from "../../features/useAuthFetch/useAuthFetch";
import "./ReputationPage.css";

import { API_BASE } from "../../config";

function fmtDateSmart(iso) {
  const d = new Date(iso);
  const now = new Date();
  const startOfToday = new Date(
    now.getFullYear(),
    now.getMonth(),
    now.getDate()
  );
  const startOfThat = new Date(d.getFullYear(), d.getMonth(), d.getDate());
  const diffDays = Math.round((startOfToday - startOfThat) / 86400000);

  if (diffDays === 0) return "today";
  if (diffDays === 1) return "yesterday";
  if (diffDays === 2) return "2 days ago";

  const opts = { month: "short", day: "numeric" };
  const s = d.toLocaleDateString(undefined, opts);
  return d.getFullYear() === now.getFullYear() ? s : `${s}, ${d.getFullYear()}`;
}
const signCls = (delta) => (delta < 0 ? "text-danger" : "text-success");
const reasonLabel = (r) => (r ?? "").toLowerCase();
const timeHM = (iso) =>
  new Date(iso).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });

export default function ReputationPage({ userId, totalReputation }) {
  const authFetch = useAuthFetch();

  const [mode, setMode] = useState("short"); // "short" | "full"
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  // SHORT
  const [shortPage, setShortPage] = useState(1);
  const [shortPageSize] = useState(30);
  const [shortList, setShortList] = useState([]);
  const [shortInfo, setShortInfo] = useState(null);

  // FULL
  const [fullPage, setFullPage] = useState(1);
  const [fullPageSize] = useState(30);
  const [fullGroups, setFullGroups] = useState([]);
  const [fullInfo, setFullInfo] = useState(null);

  // переключение режима — сброс страницы
  const switchMode = useCallback((m) => {
    setMode(m);
    if (m === "short") setShortPage(1);
    else setFullPage(1);
  }, []);

  // загрузка SHORT
  useEffect(() => {
    if (mode !== "short") return;
    let cancelled = false;
    (async () => {
      setLoading(true);
      setError("");
      try {
        const url =
          `${API_BASE}/reputations/reputation-summary-sort-list/${userId}` +
          `?page=${shortPage}&pageSize=${shortPageSize}&sortDirection=0`;
        const res = await authFetch(url);
        if (!res.ok)
          throw new Error((await res.text()) || `HTTP ${res.status}`);
        const json = await res.json();
        if (!cancelled) {
          setShortList(
            (json.value ?? []).map((x) => ({
              date: (x.occurredAt ?? "").slice(0, 10),
              delta: Number(x.delta) || 0,
            }))
          );
          setShortInfo(json.pagedInfo ?? null);
        }
      } catch (e) {
        if (!cancelled) setError(e.message ?? "Failed to load reputation");
      } finally {
        if (!cancelled) setLoading(false);
      }
    })();
    return () => {
      cancelled = true;
    };
  }, [mode, shortPage, shortPageSize, userId, authFetch]);

  // загрузка FULL
  useEffect(() => {
    if (mode !== "full") return;
    let cancelled = false;
    (async () => {
      setLoading(true);
      setError("");
      try {
        const url =
          `${API_BASE}/aggregate/get-reputation-full-list/${userId}` +
          `?page=${fullPage}&pageSize=${fullPageSize}`;
        const res = await authFetch(url);
        if (!res.ok)
          throw new Error((await res.text()) || `HTTP ${res.status}`);
        const json = await res.json();
        const rl = json.reputationList ?? json;
        if (!cancelled) {
          setFullGroups(
            (rl.value ?? []).map((g) => ({
              date: (g.date ?? "").slice(0, 10),
              delta: Number(g.delta) || 0,
              events: (g.events ?? []).map((ev) => ({
                parentId: ev.parentId,
                sourceId: ev.sourceId,
                sourceType: ev.sourceType,
                title: ev.title,
                reasonCode: ev.reasonCode,
                delta: Number(ev.delta) || 0,
                occurredAt: ev.occurredAt,
              })),
            }))
          );
          setFullInfo(rl.pagedInfo ?? null);
        }
      } catch (e) {
        if (!cancelled) setError(e.message ?? "Failed to load reputation");
      } finally {
        if (!cancelled) setLoading(false);
      }
    })();
    return () => {
      cancelled = true;
    };
  }, [mode, fullPage, fullPageSize, userId, authFetch]);

  const renderShort = () => (
    <div className="border rounded">
      <div className="list-group list-group-flush">
        {shortList.map((x, i) => (
          <div
            key={`${x.date}-${i}`}
            className="list-group-item d-flex align-items-center"
          >
            <div className={`fw-semibold me-3 ${signCls(x.delta)}`}>
              {x.delta > 0 ? `+${x.delta}` : x.delta}
            </div>
            <CaretRightFill className="text-primary me-3" />
            <div>{fmtDateSmart(x.date)}</div>
          </div>
        ))}
        {!shortList.length && (
          <div className="list-group-item text-muted">
            No reputation changes.
          </div>
        )}
      </div>

      {shortInfo && shortInfo.totalPages > 1 && (
        <div className="d-flex justify-content-end mt-2">
          <Pagination size="sm" className="mb-0">
            <Pagination.First
              onClick={() => setShortPage(1)}
              disabled={shortPage <= 1}
            />
            <Pagination.Prev
              onClick={() => setShortPage((p) => Math.max(1, p - 1))}
              disabled={shortPage <= 1}
            />
            <Pagination.Item active>{shortPage}</Pagination.Item>
            <Pagination.Next
              onClick={() =>
                setShortPage((p) => Math.min(shortInfo.totalPages, p + 1))
              }
              disabled={shortPage >= shortInfo.totalPages}
            />
            <Pagination.Last
              onClick={() => setShortPage(shortInfo.totalPages)}
              disabled={shortPage >= shortInfo.totalPages}
            />
          </Pagination>
        </div>
      )}
    </div>
  );

  const linkFor = (ev) => {
    if (ev.sourceType === "Question") return `/questions/${ev.sourceId}`;
    if (ev.sourceType === "Answer")
      return `/questions/${ev.parentId}#answer-${ev.sourceId}`;
    return undefined;
  };

  const renderFull = () => (
    <div className="border rounded">
      <div className="list-group list-group-flush">
        {fullGroups.map((g, gi) => (
          <div key={`${g.date}-${gi}`} className="list-group-item">
            <div className="d-flex align-items-center mb-2 ">
              <div className={`fw-semibold me-2 ${signCls(g.delta)}`}>
                {g.delta > 0 ? `+${g.delta}` : g.delta}
              </div>
              <CaretDownFill className="text-primary me-2" />
              <div>{fmtDateSmart(g.date)}</div>
            </div>

            <div className="rep-full__events border rounded">
              {(g.events ?? []).map((ev, ei) => {
                const href = linkFor(ev);
                return (
                  <div
                    key={`${gi}-${ei}`}
                    className="rep-full__event d-flex align-items-center py-1 small"
                  >
                    <span className="text-muted me-2">
                      {reasonLabel(ev.reasonCode === "AcceptedAnswer" ? "accepted" : ev.reasonCode)}
                    </span>
                    <span className={`fw-semibold me-2 ${signCls(ev.delta)}`}>
                      {ev.delta > 0 ? `+${ev.delta}` : ev.delta}
                    </span>
                    {href ? (
                      <a
                        className="link-primary text-decoration-none"
                        href={href}
                      >
                        {ev.title}
                      </a>
                    ) : (
                      <span>{ev.title}</span>
                    )}
                    <span className="text-muted ms-auto">
                      {timeHM(ev.occurredAt)}
                    </span>
                  </div>
                );
              })}
              {!g.events?.length && (
                <div className="text-muted small">No events.</div>
              )}
            </div>
          </div>
        ))}

        {!fullGroups.length && (
          <div className="list-group-item text-muted">
            No reputation events.
          </div>
        )}
      </div>

      {fullInfo && fullInfo.totalPages > 1 && (
        <div className="d-flex justify-content-end mt-2">
          <Pagination size="sm" className="mb-0">
            <Pagination.First
              onClick={() => setFullPage(1)}
              disabled={fullPage <= 1}
            />
            <Pagination.Prev
              onClick={() => setFullPage((p) => Math.max(1, p - 1))}
              disabled={fullPage <= 1}
            />
            <Pagination.Item active>{fullPage}</Pagination.Item>
            <Pagination.Next
              onClick={() =>
                setFullPage((p) => Math.min(fullInfo.totalPages, p + 1))
              }
              disabled={fullPage >= fullInfo.totalPages}
            />
            <Pagination.Last
              onClick={() => setFullPage(fullInfo.totalPages)}
              disabled={fullPage >= fullInfo.totalPages}
            />
          </Pagination>
        </div>
      )}
    </div>
  );

  return (
    <div>
      <div className="rep-header d-flex justify-content-between align-items-center mb-2">
        <h5 className="m-0">Reputation</h5>

        <div className="rep-controls d-flex align-items-center gap-2">
          <ButtonGroup>
            <ToggleButton
              id="rep-mode-short"
              type="radio"
              variant={mode === "short" ? "primary" : "outline-secondary"}
              checked={mode === "short"}
              onChange={() => switchMode("short")}
            >
              Short
            </ToggleButton>
            <ToggleButton
              id="rep-mode-full"
              type="radio"
              variant={mode === "full" ? "primary" : "outline-secondary"}
              checked={mode === "full"}
              onChange={() => switchMode("full")}
            >
              Full
            </ToggleButton>
          </ButtonGroup>

          {typeof totalReputation === "number" && (
            <span className="fw-semibold text-muted">
              {totalReputation.toLocaleString()} Reputation
            </span>
          )}
        </div>
      </div>

      {loading ? (
        <div className="d-flex align-items-center">
          <Spinner size="sm" className="me-2" /> Loading…
        </div>
      ) : error ? (
        <div className="text-danger small">{error}</div>
      ) : mode === "short" ? (
        renderShort()
      ) : (
        renderFull()
      )}
    </div>
  );
}

ReputationPage.propTypes = {
  userId: PropTypes.string.isRequired,
  totalReputation: PropTypes.number, // общий счёт — передай из профиля
};
