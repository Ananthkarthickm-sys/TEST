import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { frequencyAPI } from '../services/api';
import { FaEdit, FaTrash, FaSort, FaSortUp, FaSortDown, FaPlus } from "react-icons/fa";

function FrequencyList() {
  const [frequencies, setFrequencies] = useState([]);
  const [loading, setLoading] = useState(true);
  const [sortColumn, setSortColumn] = useState(null);
  const [sortOrder, setSortOrder] = useState("asc");
  const [currentPage, setCurrentPage] = useState(1);
  const rowsPerPage = 5;

  const navigate = useNavigate();

  useEffect(() => {
    fetchFrequencies();
  }, []);

  const fetchFrequencies = async () => {
    try {
      const response = await frequencyAPI.getAll();
      setFrequencies(response.data);
    } catch (err) {
      console.error('Error:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id) => {
    if (window.confirm('Delete this frequency?')) {
      try {
        await frequencyAPI.delete(id);
        fetchFrequencies();
      } catch (err) {
        alert(err.response?.data?.message || 'Failed to delete');
      }
    }
  };

  // Sorting
  const handleSort = (column) => {
    const order =
      sortColumn === column && sortOrder === "asc" ? "desc" : "asc";
    setSortColumn(column);
    setSortOrder(order);
  };

  const sortedData = [...frequencies].sort((a, b) => {
    if (!sortColumn) return 0;
    const aValue = a[sortColumn];
    const bValue = b[sortColumn];

    if (typeof aValue === "string") {
      return sortOrder === "asc"
        ? aValue.localeCompare(bValue)
        : bValue.localeCompare(aValue);
    } else {
      return sortOrder === "asc" ? aValue - bValue : bValue - aValue;
    }
  });

  // Pagination
  const indexOfLast = currentPage * rowsPerPage;
  const indexOfFirst = indexOfLast - rowsPerPage;
  const currentData = sortedData.slice(indexOfFirst, indexOfLast);
  const totalPages = Math.ceil(frequencies.length / rowsPerPage);

  const nextPage = () =>
    setCurrentPage((prev) => Math.min(prev + 1, totalPages));
  const prevPage = () =>
    setCurrentPage((prev) => Math.max(prev - 1, 1));

  const renderSortIcon = (column) => {
    if (sortColumn !== column) return <FaSort />;
    return sortOrder === "asc" ? <FaSortUp /> : <FaSortDown />;
  };

  if (loading) {
    return <div className="loading">Loading...</div>;
  }

  return (
    <div className="frequency-list-container">
      <div className="header-section">
        <h2>Payment Frequencies</h2>
        <button className="btn-add" onClick={() => navigate('/frequencies/new')}>
          <FaPlus /> Add Frequency
        </button>
      </div>

      <table className="data-table">
        <thead>
          <tr>
            <th onClick={() => handleSort("autoId")}>
              ID {renderSortIcon("autoId")}
            </th>
            <th onClick={() => handleSort("frequencyType")}>
              Description {renderSortIcon("frequencyType")}
            </th>
            <th onClick={() => handleSort("paymentsPerYear")}>
              Payment per year {renderSortIcon("paymentsPerYear")}
            </th>
            <th onClick={() => handleSort("daysBetweenPayments")}>
              Days Between {renderSortIcon("daysBetweenPayments")}
            </th>
            <th>Actions</th>
          </tr>
        </thead>

        <tbody>
          {currentData.map((freq) => (
            <tr key={freq.autoId}>
              <td>{freq.autoId}</td>
              <td>{freq.frequencyType}</td>
              <td>{freq.paymentsPerYear}</td>
              <td>{freq.daysBetweenPayments}</td>
              <td className="actions">
                <button
                  className="btn-edit"
                  onClick={() => navigate(`/frequencies/edit/${freq.autoId}`)}
                >
                  <FaEdit />
                </button>
                <button
                  className="btn-delete"
                  onClick={() => handleDelete(freq.autoId)}
                >
                  <FaTrash />
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      <div className="pagination">
        <button
          className="btn-page"
          onClick={prevPage}
          disabled={currentPage === 1}
        >
          ◀ Prev
        </button>
        <span>
          Page {currentPage} of {totalPages}
        </span>
        <button
          className="btn-page"
          onClick={nextPage}
          disabled={currentPage === totalPages}
        >
          Next ▶
        </button>
      </div>
    </div>
  );
}

export default FrequencyList;
