import React from 'react';
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import FrequencyList from './components/FrequencyList';
import FrequencyForm from './components/FrequencyForm';
import LoanList from './components/LoanList';
import LoanForm from './components/LoanForm';
import LoanCalculator from './components/LoanCalculator';
import './App.css';

function App() {
  return (
    <Router>
      <div className="app">
        <nav className="navbar">
          <div className="nav-container">
            <h1 className="nav-title">💰 Loan Management System</h1>
            <ul className="nav-links">
              <li>
                <Link to="/">Loans</Link>
              </li>
              <li>
                <Link to="/calculator">Calculator</Link>
              </li>
              <li>
                <Link to="/frequencies">Frequencies</Link>
              </li>
            </ul>
          </div>
        </nav>

        <div className="main-content">
          <Routes>
            <Route path="/" element={<LoanList />} />
            <Route path="/loans/new" element={<LoanForm />} />
            <Route path="/loans/edit/:id" element={<LoanForm />} />
            <Route path="/calculator" element={<LoanCalculator />} />
            <Route path="/frequencies" element={<FrequencyList />} />
            <Route path="/frequencies/new" element={<FrequencyForm />} />
            <Route path="/frequencies/edit/:id" element={<FrequencyForm />} />
          </Routes>
        </div>
      </div>
    </Router>
  );
}

export default App;
