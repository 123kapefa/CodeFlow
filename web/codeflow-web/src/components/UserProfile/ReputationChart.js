import React, { useEffect, useMemo, useState, useCallback } from "react";
import PropTypes from "prop-types";
import { Spinner } from "react-bootstrap";
import { useAuthFetch } from "../../features/useAuthFetch/useAuthFetch";
import { API_BASE } from "../../config";
import "./ReputationChart.css";

function formatTooltip(date, value) {
  const d = new Date(date);
  const weekday = d.toLocaleDateString(undefined, { weekday: "long" });
  const month = d.toLocaleDateString(undefined, { month: "short" });
  const day = d.toLocaleDateString(undefined, { day: "2-digit" });
  const year = d.getFullYear();
  return `${weekday}, ${month} ${day}, ${year}: ${value} reputation`;
}

// генерим последние N дат, включая сегодня (локальная дата, без времени)
function getLastNDatesDesc(n, to = new Date()) {
  const end = new Date(to.getFullYear(), to.getMonth(), to.getDate());
  return Array.from({ length: n }, (_, i) => {
    const d = new Date(end);
    d.setDate(end.getDate() - i); // 0: today, 1: yesterday, ...
    return d;
  });
}

function ymdLocal(d) {
  const y = d.getFullYear();
  const m = String(d.getMonth() + 1).padStart(2, "0");
  const day = String(d.getDate()).padStart(2, "0");
  return `${y}-${m}-${day}`;
}

export default function ReputationChart({ userId, days = 30 }) {
  const authFetch = useAuthFetch();
  const [loading, setLoading] = useState(true);
  const [serverPoints, setServerPoints] = useState([]); // как пришло с сервера
  const [error, setError] = useState("");

  // tooltip
  const [tip, setTip] = useState({ show: false, x: 0, y: 0, text: "" });

  useEffect(() => {
    let cancelled = false;

    (async () => {
      setLoading(true);
      setError("");
      try {
        const url = `${API_BASE}/reputations/reputation-month-list/${userId}`;
        const res = await authFetch(url, { method: "GET" });
        if (!res.ok)
          throw new Error((await res.text()) || `HTTP ${res.status}`);

        const data = await res.json(); // [{delta, occurredAt:"YYYY-MM-DD"}]
        if (!cancelled) {
          const arr = (Array.isArray(data) ? data : [])
            .map((x) => ({
              date: (x.occurredAt ?? "").slice(0, 10), // YYYY-MM-DD
              delta: Number(x.delta) || 0,
            }))
            .filter((x) => x.date.length === 10);
          setServerPoints(arr);
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
  }, [userId, authFetch]);

 const last = useMemo(() => getLastNDatesDesc(days).reverse(), [days]);

  // дополняем до последних N дней нулями
  const bars = useMemo(() => {
    const byDate = new Map(serverPoints.map((p) => [p.date, p.delta]));
    return last.map((d) => {
      const key = ymdLocal(d); // <-- локальный ключ
      return { date: key, delta: byDate.get(key) ?? 0 };
    });
  }, [serverPoints, last]);

  const maxAbs = useMemo(
    () => Math.max(1, ...bars.map((b) => Math.abs(b.delta))),
    [bars]
  );

  const onEnter = useCallback((e, b) => {
    const rect = e.currentTarget.getBoundingClientRect();
    setTip({
      show: true,
      x: rect.left + rect.width / 2,
      y: rect.top,
      text: formatTooltip(b.date, b.delta),
    });
  }, []);
  const onMove = useCallback((e) => {
    const rect = e.currentTarget.getBoundingClientRect();
    setTip((prev) => ({ ...prev, x: rect.left + rect.width / 2, y: rect.top }));
  }, []);
  const onLeave = useCallback(() => setTip((t) => ({ ...t, show: false })), []);

  if (loading) {
    return (
      <div className="cf-card">
        <div className="d-flex align-items-center">
          <Spinner size="sm" className="me-2" /> Loading reputation…
        </div>
      </div>
    );
  }
  if (error) {
    return (
      <div className="cf-card">
        <div className="text-danger small">{error}</div>
      </div>
    );
  }

  return (
    <div className="cf-card">
      <div className="d-flex justify-content-between align-items-center mb-2">
        <h6 className="m-0">Reputation (last {days} days)</h6>
      </div>

      <div className="rep-chart" onMouseLeave={onLeave}>
        <div className="rep-chart__baseline" />
        <div className="rep-chart__bars">
          {bars.map((b, i) => {
            const value = Math.abs(b.delta);
            const height =
              Math.round((value / maxAbs) * 80) + (value > 0 ? 4 : 0);
            const cls = b.delta < 0 ? "neg" : value === 0 ? "zero" : "pos";

            return (
              <div
                key={b.date + i}
                className={`rep-bar ${cls}`}
                style={{ height: `${height}px` }}
                onMouseEnter={(e) => onEnter(e, b)}
                onMouseMove={onMove}
                onFocus={(e) => onEnter(e, b)}
                tabIndex={0}
                aria-label={formatTooltip(b.date, b.delta)}
                title={formatTooltip(
                  b.date,
                  b.delta
                )} /* запасной нативный хинт */
              />
            );
          })}
        </div>

        {tip.show && (
          <div className="rep-tooltip" style={{ left: tip.x, top: tip.y }}>
            {tip.text}
          </div>
        )}
      </div>
    </div>
  );
}

ReputationChart.propTypes = {
  userId: PropTypes.string.isRequired,
  days: PropTypes.number,
};
