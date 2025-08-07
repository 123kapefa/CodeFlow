import React from "react";
import { useAuth } from "../../features/Auth/AuthProvider ";
import { useAuthFetch } from "../../features/useAuthFetch/useAuthFetch";
import { useNavigate, Link } from "react-router-dom";
import { useEffect } from "react";

function CreateQuestion() {
  const navigate = useNavigate();

  /* 1. Хуки всегда идут первыми */
  const { user, loading } = useAuth();
  const fetchAuth = useAuthFetch();

   useEffect(() => {
      if (!user) {
        console.log("CREATE QUESTION")
        navigate("/login");
        return; // ждём, пока появится профиль
      }}, [user, fetchAuth]);

  return <h1>Ask Вопросы</h1>;
}

export default CreateQuestion;
