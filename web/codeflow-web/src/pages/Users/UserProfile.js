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

import { RefreshToken } from "../../features/RefreshToken/RefreshToken";
import { useAuth } from "../../features/Auth/AuthProvider ";

export default function UserProfile() {
  const { userId } = useParams();
  const navigate = useNavigate();
  const { user: current } = useAuth(); // текущий залогиненный пользователь

  const [profile, setProfile] = useState(null);
  const [loading, setLoading] = useState(true);

  /* верхний переключатель: activity | settings */
  const [topTab, setTopTab] = useState("activity");

  /* внутренние вкладки для каждого раздела */
  const [activityTab, setActivity] = useState("summary");
  const [settingsTab, setSettings] = useState("edit");

  /* ---------- загрузка профиля ---------- */
  useEffect(() => {
    (async () => {
      const refresh = Cookies.get("refresh_token");
      if (!refresh) {
        navigate("/login", { replace: true });
        return;
      }

      const api = (id) =>
        fetch(`http://localhost:5000/api/users/${id}`, {
          headers: { Authorization: `Bearer ${Cookies.get("jwt")}` },
        });

      let res = await api(userId);
      if (res.status === 401 && (await RefreshToken())) res = await api(userId);

      if (res.ok) setProfile(await res.json());
      setLoading(false);
    })();
  }, [userId, navigate]);

  if (loading)
    return (
      <Container className="py-5 text-center">
        <Spinner animation="border" />
      </Container>
    );

  /* ---------- меню слева ---------- */
  const ActivityMenu = () => (
    <Nav
      variant="pills"
      className="flex-column sticky-top nav-compact"
      style={{ top: "6rem", minWidth: "120px" }}
      activeKey={activityTab}
      onSelect={setActivity}
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
      <Nav.Link eventKey="email">Edit email</Nav.Link>
      <Nav.Link eventKey="delete">Delete profile</Nav.Link>
    </Nav>
  );

  /* ---------- тело вкладок ---------- */
  const renderActivityBody = () => {
    switch (activityTab) {
      case "answers":
        return <AnswersList userId={userId} />;
      case "questions":
        return <QuestionsList userId={userId} />;
      case "tags":
        return <TagsBlock userId={userId} />;
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
        return <h5>TODO: Delete profile form</h5>;
      case "email":
        return <h5>TODO: Email settings form</h5>;
      default:
        return <h5>TODO: Edit profile form</h5>;
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

        {current?.userId === profile.userId && (
          <Button
            size="sm"
            variant={topTab === "settings" ? "warning" : "outline-secondary"}
            onClick={() => setTopTab("settings")}
          >
            Settings
          </Button>
        )}
      </div>

      <div className="row ">
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
