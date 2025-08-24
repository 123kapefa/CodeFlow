import React, { useState } from "react";
import { toast } from "react-toastify";
import { useNavigate, Link } from "react-router-dom";
import { Button, Form, Container, Row, Col, Card } from "react-bootstrap";

import { redirectToProvider } from "../../features/Auth/redirectToProvider";

import { API_BASE } from "../../config";

function Signup() {
  const [userName, setUserName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const navigate = useNavigate();

  const validatePassword = (pwd) => {
  const checks = [
    { ok: /[A-Z]/.test(pwd), msg: "Нужна минимум одна заглавная латинская буква" },
    { ok: /[a-z]/.test(pwd), msg: "Нужна минимум одна строчная латинская буква" },
    { ok: /\d/.test(pwd),  msg: "Нужна минимум одна цифра" },
    { ok: /[^A-Za-z0-9]/.test(pwd), msg: "Нужен минимум один спецсимвол (например, _ ! @ # $ ...)" },
    { ok: pwd.length >= 6, msg: "Минимум 6 символов" }
  ];
  const firstFail = checks.find(c => !c.ok);
  return firstFail ? firstFail.msg : null;
};

  const handleSubmit = async (e) => {
    e.preventDefault();

    // Clear previous error
    setError("");

    // Optional: trim whitespace from password
    const pwd = password.trim();

    const validationError = validatePassword(pwd);
    if (validationError) {
      setError(validationError);
      return; // stop submit if invalid
    }

    try {

      const response = await fetch(`${API_BASE}/auth/register`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ userName, email, password }),
      });

      if (!response.ok) {
        const err = await response.json();
        console.log(err);
        console.log(response);
        throw new Error(err.detail || "Ошибка регистрации");
      }

      // Если всё ок — переходим на главную
      toast.success("Регистрация успешна.", {
        onClose: () => navigate("/login"), // дождаться анимации
        autoClose: 1000,
      });
    } catch (err) {
      setError(err.message);
    }
  };

  return (
    <Container className="d-flex justify-content-center align-items-center">
      <Row>
        <Col>
          <div className="text-center mb-4">
            <img src="/logo/logo-transparent.png" alt="logo" height="50" />
            <h3 className="mt-2">Join CodeFlow</h3>
            <p>
              By clicking "Sign up", you agree to our{" "}
              <a href="#">terms of service</a>
              and <a href="#">privacy policy</a>.
            </p>
          </div>

          <Card className="shadow-sm p-4">
             <Button
              variant="light"
              className="mb-2 w-100 border p-0 py-2"
              onClick={() => redirectToProvider("Google")}
            >
              <i className="bi bi-google"></i> Sign up with Google
            </Button>

            <Button
              variant="dark"
              className="mb-2 w-100 p-0 py-2"
              onClick={() => redirectToProvider("GitHub")}
            >

              <i className="bi bi-github"></i> Sign up with GitHub
            </Button>

            <div className="text-center my-3">
              <span className="px-2">OR</span>
              <hr />
            </div>

            <Form onSubmit={handleSubmit}>
              {error && <div className="alert alert-danger">{error}</div>}

              <Form.Group controlId="formUsername" className="mb-3">
                <Form.Label>User Name</Form.Label>
                <Form.Control
                  type="text"
                  value={userName}
                  onChange={(e) => setUserName(e.target.value)}
                  required
                />
              </Form.Group>

              <Form.Group controlId="formEmail" className="mb-3">
                <Form.Label>Email</Form.Label>
                <Form.Control
                  type="email"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  required
                />
              </Form.Group>

              <Form.Group controlId="formPassword" className="mb-3">
                <Form.Label>Password</Form.Label>
                <Form.Control
                  type="password"
                  value={password}
                  onChange={(e) => {
                    setPassword(e.target.value);
                    if (error) setError(""); // clear message while typing
                  }}
                  placeholder="6+ characters (at least 1 letter & 1 number)"
                  required
                />
              </Form.Group>

              <Button type="submit" variant="primary" className="w-100">
                Sign up
              </Button>
            </Form>
          </Card>

          <div className="text-center mt-3">
            <p>
              Already have an account? <Link to="/login">Log in</Link>
            </p>
          </div>
        </Col>
      </Row>
    </Container>
  );
}

export default Signup;
