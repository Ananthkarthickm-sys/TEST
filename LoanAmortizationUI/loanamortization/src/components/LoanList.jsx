import React, { useState, useEffect, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import { loanAPI } from '../services/api';
import { FaEdit, FaTrash, FaPlus, FaSort, FaSortUp, FaSortDown  } from 'react-icons/fa';

import './LoanList.css';

function LoanList() {
  // 🧩 All hooks declared first
  const [loans, setLoans] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [searchTerm, setSearchTerm] = useState('');

  // Sorting + Pagination hooks MUST be here too
  const [sortConfig, setSortConfig] = useState({ key: 'title', direction: 'asc' });

  const [currentPage, setCurrentPage] = useState(1);
  const itemsPerPage = 10;

  const navigate = useNavigate();

  useEffect(() => {
    fetchLoans();
  }, []);

  const fetchLoans = async () => {
    try {
      setLoading(true);
      const response = await loanAPI.getAll();
      setLoans(response.data);
      setError(null);
    } catch (err) {
      setError('Failed to fetch loans. Please try again.');
      console.error('Error fetching loans:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id) => {
    if (window.confirm('Are you sure you want to delete this loan?')) {
      try {
        await loanAPI.delete(id);
        fetchLoans();
      } catch {
        alert('Failed to delete loan');
      }
    }
  };

  // --- Filtering ---
  const filteredLoans = loans.filter(
    (loan) =>
      loan.title?.toLowerCase().includes(searchTerm.toLowerCase()) ||
      loan.loanNumber?.toLowerCase().includes(searchTerm.toLowerCase()) ||
      loan.companyId?.toLowerCase().includes(searchTerm.toLowerCase())
  );

  // --- Sorting ---
  const sortedLoans = useMemo(() => {
    const sortable = [...filteredLoans];
    if (sortConfig.key) {
      sortable.sort((a, b) => {
        const valA = a[sortConfig.key];
        const valB = b[sortConfig.key];

        if (valA == null) return 1;
        if (valB == null) return -1;

        if (typeof valA === 'number' && typeof valB === 'number') {
          return sortConfig.direction === 'asc' ? valA - valB : valB - valA;
        }

        return sortConfig.direction === 'asc'
          ? valA.toString().localeCompare(valB.toString())
          : valB.toString().localeCompare(valA.toString());
      });
    }
    return sortable;
  }, [filteredLoans, sortConfig]);

  // --- Pagination ---
  const totalPages = Math.ceil(sortedLoans.length / itemsPerPage);
  const pagedLoans = sortedLoans.slice(
    (currentPage - 1) * itemsPerPage,
    currentPage * itemsPerPage
  );

  const handleSort = (key) => {
    setSortConfig((prev) => {
      if (prev.key === key) {
        return { key, direction: prev.direction === 'asc' ? 'desc' : 'asc' };
      }
      return { key, direction: 'asc' };
    });
  };
// Inside the LoanList component, after your hooks
const renderSortIcon = (column) => {
  if (sortConfig.key !== column) return <FaSort style={{ marginLeft: '4px' }} />;
  return sortConfig.direction === 'asc' ? (
    <FaSortUp style={{ marginLeft: '4px' }} />
  ) : (
    <FaSortDown style={{ marginLeft: '4px' }} />
  );
};
  const formatCurrency = (value) =>
    value != null
      ? `$${parseFloat(value).toLocaleString('en-US', {
          minimumFractionDigits: 2,
          maximumFractionDigits: 2,
        })}`
      : '$0.00';

  const formatDate = (dateString) =>
    dateString ? new Date(dateString).toLocaleDateString() : '';

  // 🟩 Conditional rendering AFTER hooks
  if (loading) return <div className="loading">Loading loans...</div>;
  if (error) return <div className="error">{error}</div>;

  return (
    <div className="loan-list-container">
      <div className="header-section">
        <h2>Mortgage Loan Calculator / Tracker</h2>
        <button className="btn-add" onClick={() => navigate('/loans/new')}>
          <FaPlus /> New Loan
        </button>
      </div>

      <div className="search-section">
        <input
          type="text"
          placeholder="Search loans by title, number, or company..."
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          className="search-input"
        />
      </div>

      <div className="table-container">
        <table className="data-table">
          <thead>
  <tr>
    <th>#</th>
    <th onClick={() => handleSort('title')}>
      Title {renderSortIcon('title')}
    </th>
    <th onClick={() => handleSort('loanNumber')}>
      Loan Number {renderSortIcon('loanNumber')}
    </th>
    <th onClick={() => handleSort('paymentDate')}>
      Start Date {renderSortIcon('paymentDate')}
    </th>
    <th onClick={() => handleSort('purchasePrice')}>
      Purchase Price {renderSortIcon('purchasePrice')}
    </th>
    <th onClick={() => handleSort('financedAmount')}>
      Financed {renderSortIcon('financedAmount')}
    </th>
    <th onClick={() => handleSort('frequencyType')}>
      Frequency {renderSortIcon('frequencyType')}
    </th>
    <th onClick={() => handleSort('interestRate')}>
      Rate % {renderSortIcon('interestRate')}
    </th>
    <th onClick={() => handleSort('paymentAmount')}>
      Payment {renderSortIcon('paymentAmount')}
    </th>
    <th>Actions</th>
  </tr>
</thead>

          <tbody>
            {pagedLoans.length === 0 ? (
              <tr>
                <td colSpan="12" className="no-data">
                  No loans found
                </td>
              </tr>
            ) : (
              pagedLoans.map((loan, index) => (
                <tr key={loan.autoId} className={!loan.isActive ? 'inactive-row' : ''}>
                  <td>{(currentPage - 1) * itemsPerPage + index + 1}</td> {/* ✅ Sequence */}
                 
                  {/* <td className="text-center">
                    {loan.isActive ? (
                      <FaCheckCircle className="icon-active" />
                    ) : (
                      <FaTimesCircle className="icon-inactive" />
                    )}
                  </td> */}
                  <td>{loan.title}</td>
                  <td>{loan.loanNumber}</td>
                  <td>{formatDate(loan.paymentDate)}</td>
                  <td>{formatCurrency(loan.purchasePrice)}</td>
                  <td>{formatCurrency(loan.financedAmount)}</td>
                  <td>{loan.frequencyType}</td>
                  <td>{loan.interestRate}%</td>
                  <td>{formatCurrency(loan.paymentAmount)}</td>
                  <td className="actions">
                    <button
                      className="btn-edit"
                      onClick={() => navigate(`/loans/edit/${loan.autoId}`)}
                      title="Edit"
                    >
                      <FaEdit />
                    </button>
                    <button
                      className="btn-delete"
                      onClick={() => handleDelete(loan.autoId)}
                      title="Delete"
                    >
                      <FaTrash />
                    </button>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>

        {/* Pagination Controls */}
        <div className="pagination">
          <button
            onClick={() => setCurrentPage((prev) => Math.max(prev - 1, 1))}
            disabled={currentPage === 1}
          >
            ◀ Prev
          </button>

          <span>
            Page {currentPage} of {totalPages}
          </span>

          <button
            onClick={() => setCurrentPage((prev) => Math.min(prev + 1, totalPages))}
            disabled={currentPage === totalPages}
          >
            Next ▶
          </button>
        </div>
      </div>

      <div className="record-info">
        Showing {pagedLoans.length} of {loans.length} loans
      </div>
    </div>
  );
}

export default LoanList;
