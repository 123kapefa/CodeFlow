import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Button, Form, Container, Row, Col, Card } from "react-bootstrap";
import { useAuth } from "../../features/Auth/AuthProvider ";

function Login() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
    const navigate = useNavigate();
  const { login } = useAuth();

    const handleSubmit = async (e) => {
        e.preventDefault();

    try {
      await login(email, password);
      navigate("/questions");
    } catch (err) {
      setError(err.message || "Login failed");
    }
  };

  return (
        <Container className="d-flex justify-content-center align-items-center vh-100">
      <Row>
        <Col>
          <div className="text-center mb-4">
            <img src="/logo/logo-transparent.png" alt="logo" height="50" />
          </div>
          
          <Card className="shadow-sm p-4">
            
            <Button variant="light" className="mb-2 w-100 border">
              <i className="bi bi-google"></i> Log in with Google
            </Button>
            <Button variant="dark" className="mb-2 w-100">
              <i className="bi bi-github"></i> Log in with GitHub
            </Button>           

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
                  <a href="/forgot-password">Forgot password?</a>
                </small>
              </Form.Group>

              <Button type="submit" variant="primary" className="w-100">
                Log in
              </Button>
            </Form>
          </Card>

          <div className="text-center mt-3">
            <p>
              Don’t have an account? <a href="/signup">Sign up</a>
            </p>          
          </div>
        </Col>
      </Row>
    </Container>
    );

}

export default Login;