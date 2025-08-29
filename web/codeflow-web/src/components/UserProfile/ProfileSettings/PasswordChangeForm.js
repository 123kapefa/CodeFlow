import { useState } from "react";
import { Button, Form, Alert, Spinner } from "react-bootstrap";
import Cookies from "js-cookie";
import { RefreshToken } from "../../../features/RefreshToken/RefreshToken";

import { API_BASE } from "../../../config";

export default function PasswordChangeForm({ userId }) {
  const [oldPassword, setOldPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [confirmNewPassword, setConfirmNewPassword] = useState("");
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    setSuccess("");

    if (!oldPassword || !newPassword || !confirmNewPassword) {
      setError("Please fill in all fields.");
      return;
    }

    if (newPassword !== confirmNewPassword) {
      setError("New passwords do not match.");
      return;
    }

    setLoading(true);

    const doRequest = () =>
      fetch(`${API_BASE}/auth/password-change/${userId}`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Accept: "application/json",
          Authorization: `Bearer ${Cookies.get("jwt") ?? ""}`,
        },
        body: JSON.stringify({
          oldPassword,
          newPassword,
          confirmNewPassword,
        }),
      });

    try {
      let resp = await doRequest();

      // если токен истёк, пробуем обновить
      if (resp.status === 401 && (await RefreshToken())) {
        resp = await doRequest();
      }

      if (!resp.ok) {
        const text = await resp.text();
        throw new Error(text || `HTTP ${resp.status}`);
      }

      setSuccess("Check your email to confirm the password change.");
      setOldPassword("");
      setNewPassword("");
      setConfirmNewPassword("");
    } catch (err) {
      setError(err.message || "Failed to change password");
    } finally {
      setLoading(false);
    }
  };

  return (
    <Form
      onSubmit={handleSubmit}
      className="p-3 border rounded mx-auto text-center"
    >
      <h5 className="mb-3">Change Password</h5>

      {error && <Alert variant="danger">{error}</Alert>}
      {success && <Alert variant="success">{success}</Alert>}

      <Form.Group className="mb-3" controlId="oldPassword">
        <Form.Label className="w-100 text-center">Current password</Form.Label>
        <Form.Control
          type="password"
          className="mx-auto"
          style={{ maxWidth: "300px" }}
          value={oldPassword}
          onChange={(e) => setOldPassword(e.target.value)}
        />
      </Form.Group>

      <Form.Group className="mb-3" controlId="newPassword">
        <Form.Label className="w-100 text-center">New password</Form.Label>
        <Form.Control
          type="password"
          className="mx-auto"
          style={{ maxWidth: "300px" }}
          value={newPassword}
          onChange={(e) => setNewPassword(e.target.value)}
        />
      </Form.Group>

      <Form.Group className="mb-4" controlId="confirmNewPassword">
        <Form.Label className="w-100 text-center">
          Confirm new password
        </Form.Label>
        <Form.Control
          type="password"
          className="mx-auto"
          style={{ maxWidth: "300px" }}
          value={confirmNewPassword}
          onChange={(e) => setConfirmNewPassword(e.target.value)}
        />
      </Form.Group>

      <Button type="submit" variant="primary" disabled={loading}>
        {loading ? <Spinner animation="border" size="sm" /> : "Change Password"}
      </Button>
    </Form>
  );
}
