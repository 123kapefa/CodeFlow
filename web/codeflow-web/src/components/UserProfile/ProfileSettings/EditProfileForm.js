import React, { useMemo, useState } from 'react';
import { Form, Button, Card, Row, Col, Image, Alert, Spinner } from 'react-bootstrap';
import Cookies from 'js-cookie';
import { RefreshToken } from '../../../features/RefreshToken/RefreshToken';
import { useAuth } from '../../../features/Auth/AuthProvider ';

export default function EditProfileForm({ profile, onSaved }) {
  const [username,   setUsername]   = useState(profile.userName ?? '');
  const [aboutMe,    setAboutMe]    = useState(profile.aboutMe ?? '');
  const [location,   setLocation]   = useState(profile.location ?? '');
  const [gitHubUrl,  setGitHubUrl]  = useState(profile.gitHubUrl ?? '');
  const [websiteUrl, setWebsiteUrl] = useState(profile.websiteUrl ?? '');
  const [removeAvatar, setRemoveAvatar] = useState(false);
  const [avatarFile, setAvatarFile] = useState(null);

  const [submitting, setSubmitting] = useState(false);
  const [error, setError]           = useState('');
  const [okMsg, setOkMsg]           = useState('');

  const { refreshUser } = useAuth();

  // превью: файл > objectURL, иначе текущий avatarUrl, иначе дефолт
  const previewSrc = useMemo(() => {
    if (removeAvatar) return '/avatar/avatar_default.png';
    if (avatarFile)   return URL.createObjectURL(avatarFile);
    return profile.avatarUrl?.trim() || '/avatar/avatar_default.png';
  }, [avatarFile, profile.avatarUrl, removeAvatar]);

  const handleFile = (e) => {
    const f = e.target.files?.[0];
    setAvatarFile(f || null);
    if (f) setRemoveAvatar(false);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setOkMsg('');
    setSubmitting(true);

    try {
      const form = new FormData();
      form.append('UserId', profile.userId);
      if (username?.trim()   !== '') form.append('Username',   username.trim());
      if (aboutMe?.trim()    !== '') form.append('AboutMe',    aboutMe.trim());
      if (location?.trim()   !== '') form.append('Location',   location.trim());
      if (gitHubUrl?.trim()  !== '') form.append('GitHubUrl',  gitHubUrl.trim());
      if (websiteUrl?.trim() !== '') form.append('WebsiteUrl', websiteUrl.trim());
      form.append('RemoveAvatar', removeAvatar ? 'true' : 'false');
      if (!removeAvatar && avatarFile) form.append('AvatarStream', avatarFile);

      const doPut = () =>
        fetch('http://localhost:5000/api/users/user', {
          method: 'PUT',
          // ВАЖНО: НЕ ставим Content-Type вручную, иначе сломается boundary!
          headers: {
            Authorization: `Bearer ${Cookies.get('jwt') || ''}`,
          },
          body: form,
        });

      let res = await doPut();
      if (res.status === 401 && (await RefreshToken())) res = await doPut();

      if (!res.ok) {
        const text = await res.text();
        throw new Error(text || `HTTP ${res.status}`);
      }

      setOkMsg('Профиль обновлён');
      setAvatarFile(null);
      await refreshUser();
      onSaved?.(); // попросим родителя перезагрузить профиль
    } catch (err) {
      setError(err.message || 'Ошибка при сохранении');
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <Card className="mb-4">
      <Card.Header as="h5">Edit profile</Card.Header>
      <Card.Body>
        {error && <Alert variant="danger" className="mb-3">{error}</Alert>}
        {okMsg && <Alert variant="success" className="mb-3">{okMsg}</Alert>}

        <Form onSubmit={handleSubmit}>
          <Row className="mb-3">
            <Col md="auto" className="text-center">
              <Image src={previewSrc} rounded width={96} height={96} />
            </Col>
            <Col>
              <Form.Group className="mb-2">
                <Form.Label>Avatar</Form.Label>
                <Form.Control
                  type="file"
                  accept="image/*"
                  onChange={handleFile}
                  disabled={removeAvatar}
                />
              </Form.Group>
              <Form.Check
                type="checkbox"
                label="Remove avatar"
                checked={removeAvatar}
                onChange={(e) => {
                  setRemoveAvatar(e.target.checked);
                  if (e.target.checked) setAvatarFile(null);
                }}
              />
            </Col>
          </Row>

          <Row className="g-3">
            <Col md={6}>
              <Form.Group>
                <Form.Label>Username</Form.Label>
                <Form.Control
                  value={username}
                  onChange={(e) => setUsername(e.target.value)}
                  maxLength={50}
                />
              </Form.Group>
            </Col>

            <Col md={6}>
              <Form.Group>
                <Form.Label>Location</Form.Label>
                <Form.Control
                  value={location}
                  onChange={(e) => setLocation(e.target.value)}
                  maxLength={100}
                />
              </Form.Group>
            </Col>

            <Col md={12}>
              <Form.Group>
                <Form.Label>About me</Form.Label>
                <Form.Control
                  as="textarea"
                  rows={4}
                  value={aboutMe}
                  onChange={(e) => setAboutMe(e.target.value)}
                />
              </Form.Group>
            </Col>

            <Col md={6}>
              <Form.Group>
                <Form.Label>GitHub URL</Form.Label>
                <Form.Control
                  type="url"
                  placeholder="https://github.com/username"
                  value={gitHubUrl}
                  onChange={(e) => setGitHubUrl(e.target.value)}
                />
              </Form.Group>
            </Col>

            <Col md={6}>
              <Form.Group>
                <Form.Label>Website URL</Form.Label>
                <Form.Control
                  type="url"
                  placeholder="https://example.com"
                  value={websiteUrl}
                  onChange={(e) => setWebsiteUrl(e.target.value)}
                />
              </Form.Group>
            </Col>
          </Row>

          <div className="mt-3">
            <Button type="submit" variant="primary" disabled={submitting}>
              {submitting ? (<><Spinner size="sm" className="me-2" />Saving…</>) : 'Save changes'}
            </Button>
          </div>
        </Form>
      </Card.Body>
    </Card>
  );
}