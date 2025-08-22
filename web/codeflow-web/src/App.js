import "./App.css";
import { Routes, Route } from "react-router-dom";
import { useEffect, useState } from "react";
import Cookies from "js-cookie";
import "bootstrap/dist/css/bootstrap.min.css";

import { ToastContainer } from "react-toastify";

import Header from "./components/Header/Header";
import Sidebar from "./components/Sidebar/Sidebar";
import Footer from "./components/Footer/Footer";

import Login from "./pages/Login/Login";
import Signup from "./pages/Signup/Signup";
import Logout from "./pages/Logout/Logout";

import Home from "./pages/Home/Home";

import Tags from "./pages/Tags/Tags";

import Users from "./pages/Users/Users";
import UserProfile from "./pages/Users/UserProfile";

import Questions from "./pages/Questions/Questions";
import CreateOrEditQuestion from "./pages/Questions/CreateOrEditQuestion";
import QuestionPage from "./pages/Questions/QuestionPage";

import EditAnswerPage from "./pages/Answer/EditAnswerPage";

import QuestionHistoryPage from "./pages/Questions/QuestionHistoryPage";
import AnswerHistoryPage from "./pages/Answer/AnswerHistoryPage";

import HelpPage from "./pages/Footer/CodeFlow/HelpPage";
import Teams from "./pages/Footer/Products/Teams";
import Advertising from "./pages/Footer/Products/Advertising";
import Talent from "./pages/Footer/Products/Talent";
import About from "./pages/Footer/Company/About";
import Press from "./pages/Footer/Company/Press";
import WorkHere from "./pages/Footer/Company/WorkHere";
import Legal from "./pages/Footer/Company/Legal";
import Technology from "./pages/Footer/Network/Technology";
import Culture from "./pages/Footer/Network/Culture";
import Life from "./pages/Footer/Network/Life";
import Science from "./pages/Footer/Network/Science";
import AuthCallback from "./pages/Signup/AuthCallback"

import PasswordChangeConfirm from "../src/components/UserProfile/ProfileSettings/PasswordChangeConfirm";
import EmailChangeConfirm from "../src/components/UserProfile/ProfileSettings/EmailChangeConfirm";

import RequireAuth from "../src/features/Auth/RequireAuth";

function App() {
  const [isAuthenticated, setIsAuthenticated] = useState(!!Cookies.get("jwt"));
  const [sidebarOpen, setSidebarOpen] = useState(false);

  useEffect(() => {
    const id = setInterval(
      () => setIsAuthenticated(!!Cookies.get("jwt")),
      1000
    );
    return () => clearInterval(id);
  }, []);

  useEffect(() => {
    const id = setInterval(
      () => setIsAuthenticated(!!Cookies.get("jwt")),
      1000
    );
    return () => clearInterval(id);
  }, []);

  return (
    <div className="App">
      <ToastContainer position="top-center" />

      <div className="container-xxl">
        <Header
          isAuthenticated={isAuthenticated}
          onBurgerClick={() => setSidebarOpen(true)}
        />
        <div className="main">
          <Sidebar isOpen={sidebarOpen} onClose={() => setSidebarOpen(false)} />
          {/* полупрозрачный фон для мобильного меню */}
          {sidebarOpen && (
            <div
              className="mobile-backdrop"
              onClick={() => setSidebarOpen(false)}
            />
          )}
          
          <main className="content mt-3 mb-3">
            <Routes>
              <Route path="/" element={<Questions />} />
              <Route
                path="/home"
                element={
                  <RequireAuth>
                    <Home />
                  </RequireAuth>
                }
              />
              <Route path="/signup" element={<Signup />} />
              <Route path="/login" element={<Login />} />
              <Route path="/logout" element={<Logout />} />
              <Route path="/tags" element={<Tags />} />
              <Route path="/tags/:tagId/questions" element={<Questions />} />
              <Route path="/users" element={<Users />} />
              <Route path="/users/:userId" element={<UserProfile />} />
              <Route path="/users/user_profile" element={<UserProfile />} />
              <Route path="/questions/ask" element={<CreateOrEditQuestion />} />
              <Route path="/questions/:id" element={<QuestionPage />} />
              <Route
                path="/questions/edit/:id"
                element={<CreateOrEditQuestion />}
              />
              <Route
                path="/answers/edit/:answerId"
                element={<EditAnswerPage />}
              />
              <Route
                path="/questions/:id/history"
                element={<QuestionHistoryPage />}
              />
              <Route
                path="/answers/:answerId/history"
                element={<AnswerHistoryPage />}
              />
              <Route path="/auth/callback" element={<AuthCallback />} />

              <Route path="/help" element={<HelpPage />} />
              <Route path="/teams" element={<Teams />} />
              <Route path="/advertising" element={<Advertising />} />
              <Route path="/talent" element={<Talent />} />
              <Route path="/about" element={<About />} />
              <Route path="/press" element={<Press />} />
              <Route path="/work-here" element={<WorkHere />} />
              <Route path="/legal" element={<Legal />} />
              <Route path="/technology" element={<Technology />} />
              <Route path="/culture" element={<Culture />} />
              <Route path="/life" element={<Life />} />
              <Route path="/science" element={<Science />} />

              <Route
                path="/password-change-confirm"
                element={<PasswordChangeConfirm />}
              />
              <Route
                path="/email-change-confirm"
                element={<EmailChangeConfirm />}
              />
            </Routes>
          </main>
        </div>
      </div>

      <Footer />
    </div>
  );
}

export default App;
