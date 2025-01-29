// eslint-disable-next-line no-unused-vars
import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import '../styles/Header.css';
import '../styles/Index.css';
import App from './App.jsx';
import TestUI from './TestUI';

const root = createRoot(document.getElementById('root')); // Removed unnecessary ReactDOM.createRoot duplication

root.render(
    <div>
        {/* <BackgroundListener /> */}
        <div className="header" id="fade-effect">
            <h1>Drone Situational Awareness Tool</h1>
        </div>
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<App />} />
                <Route path="/test" element={<App />} />
                <Route path="/TestUI" element={<TestUI />} />
            </Routes>
        </BrowserRouter>
    </div>
);

