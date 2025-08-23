import { useState } from "react";
import { Button, Form, Alert, Spinner } from "react-bootstrap";
import Cookies from "js-cookie";
import { RefreshToken } from "../../../features/RefreshToken/RefreshToken";

import { API_BASE } from "../../../config";

export default function EmailChangeForm({ userId }) {
  const [oldEmail, setOldEmail] = useState("");
  const [newEmail, setNewEmail] = useState("");
  const [confirmNewEmail, setConfirmNewEmail] = useState("");
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    setSuccess("");

    if (!newEmail || !confirmNewEmail) {
      setError("Please fill in all fields.");
      return;
    }
    if (newEmail !== confirmNewEmail) {
      setError("Emails do not match.");
      return;
    }

    console.log(oldEmail)

    setLoading(true);

    const doRequest = () =>
      fetch(`${API_BASE}/auth/email-change/${userId}`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Accept: "application/json",
          Authorization: `Bearer ${Cookies.get("jwt") ?? ""}`,
        },
        body: JSON.stringify({
          oldEmail,
          newEmail,
          confirmNewEmail,
        }),
      });

    try {
      let resp = await doRequest();

      if (resp.status === 401 && (await RefreshToken())) {
        resp = await doRequest();
      }

      if (!resp.ok) {
        const text = await resp.text();
        throw new Error(text || `HTTP ${resp.status}`);
      }

      setSuccess("Check your email to confirm the change.");
      setNewEmail("");
      setConfirmNewEmail("");
    } catch (err) {
      setError(err.message || "Failed to change email");
    } finally {
      setLoading(false);
    }
  };

  return (
    <Form
      onSubmit={handleSubmit}
      className="p-3 border rounded mx-auto text-center"
    >
      <h5 className="mb-3">Change Email</h5>

      {error && <Alert variant="danger">{error}</Alert>}
      {success && <Alert variant="success">{success}</Alert>}

      <Form.Group className="mb-3">
        <Form.Label className="w-100 text-center">Old email</Form.Label>
        <Form.Control
          type="email"
          className="mx-auto"
          style={{ maxWidth: "300px" }}
          value={oldEmail}
          onChange={(e) => setOldEmail(e.target.value)}
        />
      </Form.Group>

      <Form.Group className="mb-3">
        <Form.Label className="w-100 text-center">New email</Form.Label>
        <Form.Control
          type="email"
          className="mx-auto"
          style={{ maxWidth: "300px" }}
          value={newEmail}
          onChange={(e) => setNewEmail(e.target.value)}
        />
      </Form.Group>

      <Form.Group className="mb-4">
        <Form.Label className="w-100 text-center">Confirm new email</Form.Label>
        <Form.Control
          type="email"
          className="mx-auto"
          style={{ maxWidth: "300px" }}
          value={confirmNewEmail}
          onChange={(e) => setConfirmNewEmail(e.target.value)}
        />
      </Form.Group>

      <Button type="submit" variant="primary" disabled={loading}>
        {loading ? <Spinner animation="border" size="sm" /> : "Change Email"}
      </Button>
    </Form>
  );
}
