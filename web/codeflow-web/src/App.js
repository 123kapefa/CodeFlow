import './App.css';
import { Routes, Route } from 'react-router-dom';

import Header from './components/Header/Header';
import Sidebar from './components/Sidebar/Sidebar';
import Footer from './components/Footer/Footer';

import Home from './pages/Home';
import Questions from './pages/Questions';
import Tags from './pages/Tags';
import Users from './pages/Users';

function App() {
  return (
    <div className="App">
      <div className="container-xxl">
        <Header />

        <div className="main">
          <Sidebar />
          <main className="content">
            <Routes>
              <Route path="/" element={<Home />} />
              <Route path="/questions" element={<Questions />} />
              <Route path="/tags" element={<Tags />} />
              <Route path="/users" element={<Users />} />
            </Routes>
          </main>
        </div>
      </div>

      <Footer />
    </div>
  );
}

export default App;
