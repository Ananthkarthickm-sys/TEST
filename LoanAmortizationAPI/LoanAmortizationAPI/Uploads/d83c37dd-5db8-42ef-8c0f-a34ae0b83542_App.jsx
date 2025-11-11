import React from 'react';
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import DepartmentList from './components/DepartmentList';
import DepartmentForm from './components/DepartmentForm';
import EmployeeList from './components/EmployeeList';
import EmployeeForm from './components/EmployeeForm';
import './App.css';

function App() {
  return (
    <Router>
      <div className="app">
        <nav className="navbar">
          <div className="nav-container">
            <h1 className="nav-title">Employee Management System</h1>
            <ul className="nav-links">
              <li>
                <Link to="/departments">Departments</Link>
              </li>
              <li>
                <Link to="/employees">Employees</Link>
              </li>
            </ul>
          </div>
        </nav>

        <div className="main-content">
          <Routes>
            <Route path="/" element={<EmployeeList />} />
            <Route path="/departments" element={<DepartmentList />} />
            <Route path="/departments/new" element={<DepartmentForm />} />
            <Route path="/departments/edit/:deptCode" element={<DepartmentForm />} />
            <Route path="/employees" element={<EmployeeList />} />
            <Route path="/employees/new" element={<EmployeeForm />} />
            <Route path="/employees/edit/:id" element={<EmployeeForm />} />
          </Routes>
        </div>
      </div>
    </Router>
  );
}

export default App;
