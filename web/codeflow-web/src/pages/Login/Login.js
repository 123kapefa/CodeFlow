import React, { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { Button, Form, Container, Row, Col, Card } from "react-bootstrap";
import { useAuth } from "../../features/Auth/AuthProvider "; 
import { redirectToProvider } from "../../features/Auth/redirectToProvider";

export default function Login() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [redirecting, setRedirecting] = useState(null); // "Google"|"GitHub"|null
  const navigate = useNavigate();
  const { login } = useAuth();

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      await login(email, password);
      navigate("/");
    } catch (err) {
      setError(err?.message || "Login failed");
    }
  };

  const socialLogin = (provider) => {
    setRedirecting(provider);
    redirectToProvider(provider, "/");
  };

  return (
    <Container className="d-flex justify-content-center align-items-center vh-100">
      <Row>
        <Col>
          <div className="text-center mb-4">
            <img src="/logo/logo-transparent.png" alt="logo" height="50" />
          </div>

          <Card className="shadow-sm p-4">
            <Button
              variant="light"
              className="mb-2 w-100 border"
              disabled={!!redirecting}
              onClick={() => socialLogin("Google")}
            >
              <i className="bi bi-google" />{" "}
              {redirecting === "Google" ? "Redirecting…" : "Log in with Google"}
            </Button>

            <Button
              variant="dark"
              className="mb-2 w-100"
              disabled={!!redirecting}
              onClick={() => socialLogin("GitHub")}
            >
              <i className="bi bi-github" />{" "}
              {redirecting === "GitHub" ? "Redirecting…" : "Log in with GitHub"}
            </Button>

            <div className="text-center my-3">
              <span className="px-2">OR</span>
              <hr />
            </div>

            <Form onSubmit={handleSubmit}>
              {error && <div className="alert alert-danger">{error}</div>}

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
                  onChange={(e) => setPassword(e.target.value)}
                  required
                />
                <small className="text-muted">
                  <Link to="/forgot-password">Forgot password?</Link>
                </small>
              </Form.Group>

              <Button type="submit" variant="primary" className="w-100">
                Log in
              </Button>
            </Form>
          </Card>

          <div className="text-center mt-3">
            <p>
              Don’t have an account? <Link to="/signup">Sign up</Link>
            </p>
          </div>
        </Col>
      </Row>
    </Container>
  );
}