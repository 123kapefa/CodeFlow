import Cookies from 'js-cookie';


export async function RefreshToken() {

  const refreshToken = Cookies.get('refresh_token');
  if (!refreshToken) return false;

  try {
    const resp = await fetch(`http://localhost:5000/api/auth/refresh-token`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json', Accept: 'application/json' },                   
      body: JSON.stringify( refreshToken ) 
    });

    if (!resp.ok) return false;

    const data = await resp.json(); // { accessToken, refreshToken, expiresInSeconds }

    
    const accessDays = data.expiresInSeconds ? data.expiresInSeconds / 86400 : 1 / 24; 

    Cookies.set('jwt', data.accessToken, {
      expires: accessDays,
      path: '/',
      sameSite: 'Lax',
      // secure: true   
    });

    if (data.refreshToken) {
      Cookies.set('refresh_token', data.refreshToken, {
        expires: 30,         
        path: '/',
        sameSite: 'Lax'
        // secure: true
      });
    }

    return true;
  } 
  catch {
    return false;
  }
}