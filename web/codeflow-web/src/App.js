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
import CreateQuestion from "./pages/Questions/CreateQuestion";
import QuestionPage from "./pages/Questions/QuestionPage";

function App() {  
  const [isAuthenticated, setIsAuthenticated] = useState(!!Cookies.get("jwt"));

  // На случай, если кука была выставлена/удалена где-то снаружи:
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
        <Header isAuthenticated={isAuthenticated} />

        <div className="main">
          <Sidebar />
          <main className="content mt-2 mb-2">
            <Routes>
              <Route path="/" element={<Home />} />
              <Route path="/signup" element={<Signup />} />
              <Route path="/login" element={<Login />} />
              <Route path="/logout" element={<Logout />} />
              <Route path="/questions" element={<Questions />} />
              <Route path="/tags/:tagId/questions" element={<Questions />} />
              <Route path="/tags" element={<Tags />} />
              <Route path="/users" element={<Users />} />
              <Route path="/users/:userId" element={<UserProfile />} />
              <Route path="/users/user_profile" element={<UserProfile />} />
              <Route path="/questions/ask" element={<CreateQuestion />} />
              <Route path="/questions/:id" element={<QuestionPage />} />
            </Routes>
          </main>
        </div>
      </div>

      <Footer />
    </div>
  );
}

export default App;
