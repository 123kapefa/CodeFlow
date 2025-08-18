import React, { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { Container, Spinner, Nav, Button } from "react-bootstrap";
import Cookies from "js-cookie";

import ProfileHeader from "../../components/UserProfile/ProfileHeader";
import AnswersList from "../../components/UserProfile/AnswersList";
import QuestionsList from "../../components/UserProfile/QuestionsList";
import TagsBlock from "../../components/UserProfile/TagsBlock";
import ReputationChart from "../../components/UserProfile/ReputationChart";
import AboutBlock from "../../components/UserProfile/AboutBlock";
import PasswordChangeForm from "../../components/UserProfile/ProfileSettings/PasswordChangeForm";
import EmailChangeForm from "../../components/UserProfile/ProfileSettings/EmailChangeForm";

import QuestionsSummaryPage from "../Questions/QuestionsSummaryPage"
import AnswersSummaryPage from "../Answer/AnswersSummaryPage";
import TagsSummaryPage from "../Tags/TagsSummaryPage"

import { RefreshToken } from "../../features/RefreshToken/RefreshToken";
import { useAuth } from "../../features/Auth/AuthProvider ";
import EditProfileForm from "../../components/UserProfile/ProfileSettings/EditProfileForm";

import { API_BASE } from "../../config";

export default function UserProfile() {
  const { userId } = useParams();
  const { user: current } = useAuth(); // текущий залогиненный пользователь

  const [profile, setProfile] = useState(null);
  const [loading, setLoading] = useState(true);

  /* верхний переключатель: activity | settings */
  const [topTab, setTopTab] = useState("activity");

  /* внутренние вкладки для каждого раздела */
  const [activityTab, setActivity] = useState("summary");
  const [settingsTab, setSettings] = useState("edit");

  const navigate = useNavigate();

  const reloadProfile = async () => {
    const res = await fetch(`${API_BASE}/users/${userId}`, {
      headers: { Authorization: `Bearer ${Cookies.get("jwt")}` },
    });
    if (res.ok) setProfile(await res.json());
  };

  /* ---------- загрузка профиля ---------- */
  useEffect(() => {
    let cancelled = false;

    (async () => {
      try {
        const access = Cookies.get("jwt");

        const fetchUser = (authorized) =>
          fetch(`${API_BASE}/users/${userId}`, {
            headers: authorized
              ? { Authorization: `Bearer ${Cookies.get("jwt")}` }
              : {},
          });

        let res;

        if (access) {
          // пробуем авторизованно (для своего профиля видны приватные поля)
          res = await fetchUser(true);

          // если 401 — попытаемся рефрешнуть и повторить
          if (res.status === 401 && (await RefreshToken())) {
            res = await fetchUser(true);
          }

          // если всё ещё 401 — откат на публичный запрос
          if (res.status === 401) {
            res = await fetchUser(false);
          }
        } else {
          // нет токена — публичный просмотр
          res = await fetchUser(false);
        }

        if (!cancelled) {
          if (res.ok) {
            const json = await res.json();
            setProfile(json);
          } else {
            setProfile(null);
          }
        }
      } catch {
        if (!cancelled) setProfile(null);
      } finally {
        if (!cancelled) setLoading(false);
      }
    })();

    return () => {
      cancelled = true;
    };
  }, [userId]);

  if (loading) {
    return (
      <Container className="py-5 text-center">
        <Spinner animation="border" />
      </Container>
    );
  }

  if (!profile) {
    return (
      <Container className="py-5">
        <h5>User not found</h5>
      </Container>
    );
  }

  /* ---------- вычисления владельца ---------- */
  const ownerId = profile.id ?? profile.userId ?? null;
  const currentId = current?.id ?? current?.userId ?? null;
  const isOwner = !!(ownerId && currentId && ownerId === currentId);

  /* ---------- меню слева ---------- */
  const ActivityMenu = () => (
    <Nav
      variant="pills"
      className="flex-column sticky-top nav-compact"
      style={{ top: "6rem", minWidth: "120px" }}
      activeKey={activityTab}
      onSelect={(key) => setActivity(key)}
    >
      <Nav.Link eventKey="summary">Summary</Nav.Link>
      <Nav.Link eventKey="answers">Answers</Nav.Link>
      <Nav.Link eventKey="questions">Questions</Nav.Link>
      <Nav.Link eventKey="tags">Tags</Nav.Link>
      <Nav.Link eventKey="reputation">Reputation</Nav.Link>
    </Nav>
  );

  const SettingsMenu = () => (
    <Nav
      variant="pills"
      className="flex-column sticky-top nav-compact"
      style={{ top: "6rem", minWidth: "120px" }}
      activeKey={settingsTab}
      onSelect={setSettings}
    >
      <Nav.Link eventKey="edit">Edit profile</Nav.Link>
      <Nav.Link eventKey="security">Security</Nav.Link>
      <Nav.Link eventKey="delete">Delete profile</Nav.Link>
    </Nav>
  );

  // УДАЛЕНИЕ ПОЛЬЗОВАТЕЛЯ
  function DeleteAccountForm({ ownerId, userName, onDeleted }) {
    const [confirmText, setConfirmText] = useState("");
    const [agree, setAgree] = useState(false);
    const [busy, setBusy] = useState(false);
    const [error, setError] = useState("");


    const canDelete = agree && confirmText.trim() === (userName ?? "");

    const doDelete = async () => {
      setError("");
      setBusy(true);

      const call = () =>
        fetch(`${API_BASE}/auth/${ownerId}`, {
          method: "DELETE",
          headers: {
            Accept: "application/json",
            Authorization: `Bearer ${Cookies.get("jwt") ?? ""}`,
          },
          credentials: "include",
        });

      try {
        let resp = await call();

        // пробуем обновить access токен при 401
        if (resp.status === 401 && (await RefreshToken())) {
          resp = await call();
        }

        if (!resp.ok) {
          const msg = (await resp.text()) || `HTTP ${resp.status}`;
          throw new Error(msg);
        }

        onDeleted?.(); // уведомим родителя
      } catch (e) {
        setError(e.message ?? "Failed to delete account");
      } finally {
        setBusy(false);
      }
    };

    return (
      <div className="p-4 border rounded text-center">
        <h5 className="text-danger m-3">Delete your account</h5>

        <p className="mb-2">
          This action <strong>permanently</strong> removes your profile and
          cannot be undone.
        </p>

        <div className="form-check centered mb-3">
          <input
            id="agree"
            className="form-check-input"
            type="checkbox"
            checked={agree}
            onChange={(e) => setAgree(e.target.checked)}
          />
          <label htmlFor="agree" className="form-check-label m-2">
            I understand this is irreversible.
          </label>
        </div>

        <div className="mb-3">
          <label className="form-label">
            <code>To confirm, type your username!</code>
          </label>
          <input
            className="form-control text-center mx-auto"
            style={{ maxWidth: "50%" }}
            value={confirmText}
            onChange={(e) => setConfirmText(e.target.value)}
            placeholder={userName}
          />
        </div>

        {error && <div className="alert alert-danger py-2">{error}</div>}

        <Button
          className="m-4"
          variant="danger"
          disabled={!canDelete || busy}
          onClick={doDelete}
        >
          {busy ? "Deleting..." : "Delete profile"}
        </Button>
      </div>
    );
  }

  /* ---------- тело вкладок ---------- */
  const renderActivityBody = () => {
    switch (activityTab) {
      case "answers":
        return <AnswersSummaryPage userId={userId} />;
      case "questions":
        return <QuestionsSummaryPage userId={userId} />;
      case "tags":
        return <TagsSummaryPage userId={userId}/>;
      case "reputation":
        return <ReputationChart userId={userId} />;
      default:
        return (
          <>
            {/* About – показываем только если aboutMe не пустой */}
            {profile.aboutMe?.trim() && <AboutBlock text={profile.aboutMe} />}

            <div className="row g-4">
              <div className="col-lg-6">
                <AnswersList userId={userId} />
              </div>
              <div className="col-lg-6">
                <QuestionsList userId={userId} />
              </div>
              <div className="col-lg-6">
                <TagsBlock userId={userId} />
              </div>
              <div className="col-lg-6">
                <ReputationChart userId={userId} />
              </div>
            </div>
          </>
        );
    }
  };

  const renderSettingsBody = () => {
    switch (settingsTab) {
      case "delete":
        return (
          <DeleteAccountForm
            ownerId={ownerId}
            userName={profile.userName}
            onDeleted={async () => {
              // выходим из аккаунта и уводим на главную
              try {
                // локальный логаут — без отправки JWT в заголовках
                Cookies.remove("jwt", { path: "/" });
                Cookies.remove("refresh_token", { path: "/" });
              } finally {
                navigate("/"); // домой
              }
            }}
          />
        );
      case "security":
        return (
          <>
            <PasswordChangeForm userId={ownerId} />
            <div className="my-4" />
            <EmailChangeForm userId={ownerId} />
          </>
        );
      default:
        return <EditProfileForm profile={profile} onSaved={reloadProfile} />;
    }
  };

  /* ---------- разметка ---------- */
  return (
    <Container className="py-4">
      <ProfileHeader profile={profile} />

      {/* кнопки Activity / Settings */}
      <div className="mb-3 d-flex justify-content-start">
        <Button
          size="sm"
          variant={topTab === "activity" ? "warning" : "outline-secondary"}
          className="me-2"
          onClick={() => setTopTab("activity")}
        >
          Activity
        </Button>

        {isOwner && (
          <Button
            size="sm"
            variant={topTab === "settings" ? "warning" : "outline-secondary"}
            onClick={() => setTopTab("settings")}
          >
            Settings
          </Button>
        )}
      </div>

      <div className="row">
        {/* левое меню */}
        <div className="col-lg-2 col-md-3 mb-4">
          {topTab === "activity" ? <ActivityMenu /> : <SettingsMenu />}
        </div>

        {/* контент */}
        <div className="col-lg-10 col-md-9">
          {topTab === "activity" ? renderActivityBody() : renderSettingsBody()}
        </div>
      </div>
    </Container>
  );
}
