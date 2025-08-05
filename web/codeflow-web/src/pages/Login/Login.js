import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import Cookies from 'js-cookie';
import { Button, Form, Container, Row, Col, Card } from 'react-bootstrap';
import { jwtDecode } from "jwt-decode";

function Login() {

    const[email, setEmail] = useState('');
    const[password, setPassword] = useState('');
    const[error, setError] = useState('');
    const navigate = useNavigate();

    const handleSubmit = async (e) => {
        e.preventDefault();

        try{
            // Запрос в AuthService
            const response = await fetch('http://localhost:5000/api/auth/login', {
                method: 'POST',
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({email, password})
            });

            if(!response.ok){
                throw new Error('Неверный email или пароль');
            }

            const data = await response.json();
           
            console.log(data.accessToken);
            console.log(data.refreshToken);

            const claims = jwtDecode(data.accessToken);
            console.log(claims);

            Cookies.set("userId",claims.sub);
            
             // Сохраняем JWT в куки
            // Cookies.set('jwt', data.accessToken, { secure: true, sameSite: 'Strict' });
            Cookies.set('jwt', data.accessToken, { expires: 7, secure: true });
            Cookies.set('refresh_token', data.refreshToken, { secure: true, sameSite: 'Strict' });
            
            console.log("USER ID ", Cookies.get("userId"))

            // Перенаправление на главную
            navigate('/');
        }
        catch(err){
            setError(err.massage);
        }
    }

    return(
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