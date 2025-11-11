import React, { useState, useMemo } from 'react';
import './PaymentScheduleTable.css';

function PaymentScheduleTable({ loanId, schedule }) {
  const [sortConfig, setSortConfig] = useState({ key: 'paymentNumber', direction: 'asc' });
  const [currentPage, setCurrentPage] = useState(1);
  const itemsPerPage = 10;

  const formatCurrency = (value) => (value != null ? `$${parseFloat(value).toFixed(2)}` : '');
  const formatDate = (dateString) => (dateString ? new Date(dateString).toLocaleDateString() : '');

  // --- Sorting ---
  const sortedSchedule = useMemo(() => {
    const sortable = [...schedule];
    if (sortConfig.key) {
      sortable.sort((a, b) => {
        const valA = a[sortConfig.key];
        const valB = b[sortConfig.key];

        if (valA === null || valA === undefined) return 1;
        if (valB === null || valB === undefined) return -1;

        if (typeof valA === 'number' && typeof valB === 'number') {
          return sortConfig.direction === 'asc' ? valA - valB : valB - valA;
        }

        return sortConfig.direction === 'asc'
          ? valA.toString().localeCompare(valB.toString())
          : valB.toString().localeCompare(valA.toString());
      });
    }
    return sortable;
  }, [schedule, sortConfig]);

  // --- Pagination ---
  const totalPages = Math.ceil(sortedSchedule.length / itemsPerPage);
  const pagedSchedule = sortedSchedule.slice(
    (currentPage - 1) * itemsPerPage,
    currentPage * itemsPerPage
  );

  // --- Handle sorting toggle ---
  const handleSort = (key) => {
    setSortConfig((prev) => ({
      key,
      direction: prev.key === key && prev.direction === 'asc' ? 'desc' : 'asc'
    }));
  };

  return (
    <div className="payment-schedule-container">
      <h3>Payment Schedule</h3>
      <div className="schedule-table-wrapper">
        <table className="schedule-table">
          <thead>
            <tr>
              <th onClick={() => handleSort('paymentNumber')}>Pay Num.</th>
              <th onClick={() => handleSort('scheduleDate')}>Date</th>
              <th onClick={() => handleSort('startingBalance')}>Starting $</th>
              <th onClick={() => handleSort('interestRatePerPeriod')}>Rate %</th>
              <th onClick={() => handleSort('paymentAmount')}>Payment</th>
              <th onClick={() => handleSort('principal')}>Principal</th>
              <th onClick={() => handleSort('interest')}>Interest</th>
              <th onClick={() => handleSort('endingBalance')}>Ending $</th>
              <th onClick={() => handleSort('loanExtraPaymentPerPeriod')}>Extra Payment</th>
              <th onClick={() => handleSort('endingBalanceExtra')}>Ending $ Extra</th>
            </tr>
          </thead>
          <tbody>
            {pagedSchedule.map((payment) => (
              <tr key={payment.autoId} className={payment.paymentNumber === 0 ? 'initial-payment' : ''}>
                <td>{payment.paymentNumber}</td>
                <td>{formatDate(payment.scheduleDate)}</td>
                <td>{formatCurrency(payment.startingBalance)}</td>
                <td>{payment.interestRatePerPeriod ? payment.interestRatePerPeriod.toFixed(2) + '%' : ''}</td>
                <td>{formatCurrency(payment.paymentAmount)}</td>
                <td>{formatCurrency(payment.principal)}</td>
                <td>{formatCurrency(payment.interest)}</td>
                <td>{formatCurrency(payment.endingBalance)}</td>
                <td>{formatCurrency(payment.loanExtraPaymentPerPeriod)}</td>
                <td>{formatCurrency(payment.endingBalanceExtra)}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* Pagination Controls */}
      <div className="pagination">
        <button onClick={() => setCurrentPage((prev) => Math.max(prev - 1, 1))} disabled={currentPage === 1}>
          ◀ Prev
        </button>
        <span>
          Page {currentPage} of {totalPages}
        </span>
        <button onClick={() => setCurrentPage((prev) => Math.min(prev + 1, totalPages))} disabled={currentPage === totalPages}>
          Next ▶
        </button>
      </div>
       <div className="record-info">
        Showing {pagedSchedule.length} of {schedule.length} loans
      </div>
    </div>
  );
}

export default PaymentScheduleTable;
