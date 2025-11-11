import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { loanAPI, frequencyAPI, paymentScheduleAPI } from '../services/api';
import { FaSave, FaTimes, FaCalculator, FaListAlt, FaChartArea } from 'react-icons/fa';
import PaymentScheduleTable from './PaymentScheduleTable';
import LoanChart from './LoanChart';
import './LoanForm.css';
import AttachmentPopup from "./AttachmentPopup";

function LoanForm() {
  const [showPopup, setShowPopup] = useState(false);
  const [selectedLoanId, setSelectedLoanId] = useState(101); // Example loan ID
  const [formData, setFormData] = useState({
    companyId: '',
    associatedAccountId: '',
    isActive: true,
    ordering: 1,
    title: '',
    loanNumber: '',
    paymentDate: new Date().toISOString().split('T')[0],
    purchasePrice: '',
    cashDown: '',
    frequency: '',
    numberYears: '0',
    interestRate: '',
    correctionRate: '',
    periodInterestRate: '',
    finalPeriodRate: '',
    notes: '',
    signedTermYears: '0',
    signedTermType: 'Fixed',
    signedMaturityDate: new Date().toISOString().split('T')[0],
    signedTermNumberOfPayments: '',
    signedTermRemaining: '',
    extraPaymentTrigger: false,
    paymentAmount: '',
    additionPaymentAmount: '',
    newTermLength: '',
    newNumberOfPayments: '',
    loanExtraPayment: '',
    annualTaxAmount: '',
    periodTaxAmount: ''
  });

  const [loanDetails, setLoanDetails] = useState(null);
  const [frequencies, setFrequencies] = useState([]);
  const [schedule, setSchedule] = useState([]);
  const [showSchedule, setShowSchedule] = useState(false);
  const [showChart, setShowChart] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [message, setMessage] = useState(null);

  const navigate = useNavigate();
  const { id } = useParams();
  const isEditMode = Boolean(id);

  useEffect(() => {
    fetchFrequencies();
    if (isEditMode) {
      fetchLoan();
    }
  }, [id]);

  const fetchFrequencies = async () => {
    try {
      const response = await frequencyAPI.getAll();
      setFrequencies(response.data);
    } catch (err) {
      console.error('Error fetching frequencies:', err);
    }
  };

  const fetchLoan = async () => {
    try {
      debugger;
      setLoading(true);
      const response = await loanAPI.getById(id);
      const data = response.data;
      setFormData({
        companyId: data.companyId || '',
        associatedAccountId: data.associatedAccountId || '',
        isActive: data.isActive,
        ordering: data.ordering || 1,
        title: data.title || '',
        loanNumber: data.loanNumber || '',
        paymentDate: data.paymentDate ? data.paymentDate.split('T')[0] : '',
        purchasePrice: data.purchasePrice || '',
        cashDown: data.cashDown || '',
        frequency: data.frequency || '',
        numberYears: data.numberYears || '',
        interestRate: data.interestRate || '',
        correctionRate: data.correctionRateStr || '0',
        periodInterestRate: data.periodInterestRate || '0',
        periodInterestRate: data.periodInterestRateStr || '0',
        finalPeriodRate: data.finalInterestRate || '0',
        finalPeriodRate: data.finalInterestRateStr || '0',
        notes: data.notes || '',
        signedTermYears: data.signedTermYears || '',
        signedTermType: data.signedTermType || 'Fixed',
        signedMaturityDate: data.signedMaturityDate ? data.signedMaturityDate.split('T')[0] : '',
        signedTermNumberOfPayments: data.signedTermNumberOfPayments || '',
        signedTermRemaining: data.signedTermRemaining || '',
        paymentAmount: data.paymentAmount || '',
        newTermLength: data.newTermLength || '',
        newNumberOfPayments: data.newNumberOfPayments || '',
        extraPaymentTrigger: data.extraPaymentTrigger || false,
        loanExtraPayment: data.loanExtraPayment || '',
        annualTaxAmount: data.annualTaxAmount || '',
        periodTaxAmount: data.periodTaxAmount || ''
      });

      setLoanDetails(data);
    } catch (err) {
      setError('Failed to fetch loan details');
      console.error('Error fetching loan:', err);
    } finally {
      setLoading(false);
    }
  };

  const fetchSchedule = async () => {
    try {
      const response = await paymentScheduleAPI.getByLoan(id);
      setSchedule(response.data);
    } catch (err) {
      console.error('Error fetching schedule:', err);
    }
  };
  // Helper function to convert currency string to number
  const parseCurrency = (value) => {
    console.log("parseCurrency input:", value, typeof value);
    if (!value) return 0;
    return parseFloat(value.toString().replace(/[^0-9.-]+/g, "")) || 0;
  };
  // Helper function to convert currency string to number
  const parsePercentage = (value) => {
    if (value === null || value === undefined) return 0;
    const cleaned = value.toString().replace(/[^0-9.-]/g, '');
    const parsed = parseFloat(cleaned);
    return isNaN(parsed) ? 0 : parsed; // 70000 -> 0.07
  };

  const [displayRate, setDisplayRate] = useState(() => {
    // Initialize display value based on existing formData
    return formData.interestRate
      ? `${(formData.interestRate * 100).toFixed(4)}%`
      : '';
  });

  useEffect(() => {
    // Only update display when formData.interestRate changes externally
    setDisplayRate(
      formData.interestRate
        ? `${(formData.interestRate).toFixed(4)}%`
        : ''
    );
  }, [formData.interestRate]);

  const handleRateChange = (e) => {
    const raw = e.target.value.replace('%', '');
    setDisplayRate(raw); // allow typing freely
  };

  const handleRateBlur = () => {
    const numeric = parsePercentage(displayRate); // e.g. 70000 -> 0.07
    setFormData(prev => ({ ...prev, interestRate: numeric }));

    // Show formatted percentage (7.0000%)
    setDisplayRate(`${(numeric).toFixed(4)}%`);
    handleCalculateLoan();
  };


  const AnnualTaxhandleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData(prev => {
      const updated = {
        ...prev,
        [name]: type === 'checkbox' ? checked : value
      };


      formData.periodTaxAmount = value;

      return updated;
    });
  };
  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;

    setFormData(prev => {
      const updated = {
        ...prev,
        [name]: type === 'checkbox' ? checked : value
      };
      //console.log('selectedFrequency' + selectedFrequency.paymentsPerYear);
      // Only call when both purchasePrice & cashDown have values
      const purchaseValue = parseCurrency(updated.purchasePrice);
      const purchase = parseFloat(purchaseValue) || 0;
      const cashValue = parseCurrency(updated.cashDown);
      const cash = parseFloat(cashValue) || 0;

      if (purchase > 0 && cash > 0) {
        console.log('Calling loan calculation...');
        // handleCalculateLoan(); // ✅ <-- CALL IT HERE
      }

      console.log('Updated financed amount:', purchase - cash);

      return updated;
    });
  };


  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError(null);
    setMessage(null);

    try {
      const submitData = {
        ...formData,
        purchasePrice: parseFloat(parseCurrency(formData.purchasePrice)) || null,
        cashDown: parseFloat(parseCurrency(formData.cashDown)) || null,
        frequency: parseInt(formData.frequency) || null,
        numberYears: parseFloat(formData.numberYears) || null,
        interestRate: parseFloat(formData.interestRate) || null,
        correctionRate: parseFloat(formData.correctionRate) || 0,
        signedTermYears: parseFloat(formData.signedTermYears) || null,
        loanExtraPayment: parseFloat(parseCurrency(formData.loanExtraPayment)) || 0,
        annualTaxAmount: parseFloat(parseCurrency(formData.annualTaxAmount)) || 0,
        periodTaxAmount: parseFloat(parseCurrency(formData.periodTaxAmount)) || 0,
        ordering: parseInt(formData.ordering) || 1
      };

      if (isEditMode) {
        await loanAPI.update(id, submitData);
        setMessage('Loan updated successfully!');
      } else {
        const response = await loanAPI.create(submitData);
        setMessage('Loan created successfully!');
        navigate(`/loans/edit/${response.data.autoId}`);
      }
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to save loan');
      console.error('Error saving loan:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleCalculateLoan = async () => {
    setLoading(true);
    setError(null);
    setMessage(null);

    try {
      const submitData = {
        // Map frontend form fields correctly to backend expectations      
        purchasePrice: parseFloat(parseCurrency(formData.purchasePrice)) || 0,
        cashDown: parseFloat(parseCurrency(formData.cashDown)) || 0,
        frequencyId: parseInt(formData.frequency) || 0, // ✅ FIXED
        numberYears: parseInt(formData.numberYears) || 0,
        interestRate: parseFloat(parsePercentage(formData.interestRate)) || 0,
        correctionRate: parseFloat(formData.correctionRate) || 0,
        loanExtraPayment: parseFloat(parseCurrency(formData.loanExtraPayment)) || 0,
        annualTaxAmount: parseFloat(parseCurrency(formData.annualTaxAmount)) || 0,
        ordering: parseInt(formData.ordering) || 1
      };

      const response = await loanAPI.calculate(submitData);
      setLoanDetails(response.data);
      setFormData(prev => ({
        ...prev,
        periodInterestRate: response.data.periodInterestRateStr || '0',
        finalPeriodRateStr: response.data.finalInterestRateStr || '0',
      }));
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to calculate loan');
      console.error('Error calculating loan:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleCalculate = async () => {
    if (!id) {
      setError('Please save the loan first before calculating');
      return;
    }

    setLoading(true);
    setError(null);
    setMessage(null);

    try {
      const response = await loanAPI.calculateFull(id);
      setLoanDetails(response.data);
      debugger;
      setMessage('Loan calculated successfully! Check the Results section.');
      fetchLoan(); // Refresh the form data
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to calculate loan');
      console.error('Error calculating loan:', err);
    } finally {
      setLoading(false);
    }
  };



  const handleGenerateSchedule = async () => {
    if (!id) {
      setError('Please save the loan first before generating schedule');
      return;
    }

    setLoading(true);
    setError(null);
    setMessage(null);

    try {
      const response = await loanAPI.generateSchedule(id);
      setMessage(response.data.message);
      fetchSchedule();
      fetchLoan(); // Refresh to get updated totals
      setShowSchedule(true);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to generate schedule');
      console.error('Error generating schedule:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleToggleSchedule = () => {
    if (!showSchedule && schedule.length === 0) {
      fetchSchedule();
    }
    setShowSchedule(!showSchedule);
  };

  const handleToggleChart = () => {
    setShowChart(!showChart);
  };

  if (loading && isEditMode && !loanDetails) {
    return <div className="loading">Loading loan...</div>;
  }
  const handleFocus = (e) => {
    const value = e.target.value.replace(/[^0-9.]/g, "");
    debugger;
  };

  //const periodTaxAmount = (formData.annualTaxAmount || '$0.00');
  const financedAmount = (parseFloat(parseCurrency(formData.purchasePrice)) || 0) - (parseFloat(parseCurrency(formData.cashDown)) || 0);
  const selectedFrequency = frequencies.find(f => f.autoId === parseInt(formData.frequency));

  return (
    <div className="loan-form-container">
      <div className="form-header">
        <h2>{isEditMode ? 'Edit Loan' : 'Create New Loan'}</h2>
        {isEditMode && loanDetails && (
          <div className="loan-id">ID: {loanDetails.autoId}</div>
        )}
      </div>

      {error && <div className="error-message">{error}</div>}
      {message && <div className="success-message">{message}</div>}

      <form onSubmit={handleSubmit} className="loan-form">
        <div className="form-grid">
          {/* Long Term Information Section */}
          <div className="form-section">
            <h3 className="section-title">Mortgage - Long Term Information</h3>

            <div className="form-row">
              <div className="form-group">
                <label htmlFor="title">Title</label>
                <input
                  type="text"
                  id="title"
                  name="title"
                  value={formData.title}
                  onChange={handleChange}
                  placeholder="House Loan"
                />
                <label htmlFor="isActive">
                  <input
                    type="checkbox"
                    id="isActive"
                    name="isActive"
                    checked={formData.isActive}
                    onChange={handleChange}
                  />
                  {' '}Loan is active
                </label>
              </div>
              <div className="form-group">
                <label htmlFor="loanNumber">Loan Number</label>
                <input
                  type="text"
                  id="loanNumber"
                  name="loanNumber"
                  value={formData.loanNumber}
                  onChange={handleChange}
                  placeholder="Loan Number"
                />
              </div>
            </div>
            <div className="form-row">
              <div className="form-group">
                <label htmlFor="purchasePrice">Purchase Price *</label>
                <input
                  type="text"
                  id="purchasePrice"
                  name="purchasePrice"
                  value={formData.purchasePrice.toLocaleString('en-US', { style: 'currency', currency: 'USD' }) || '$0.00'}
                  onChange={handleChange}
                  required
                  placeholder="$0.00"
                />
              </div>
              <div className="form-group">
                <label htmlFor="paymentDate">Start Date</label>
                <input
                  type="date"
                  id="paymentDate"
                  name="paymentDate"
                  value={formData.paymentDate}
                  onChange={handleChange}
                  required
                />
              </div>
            </div>

            <div className="form-row">
              <div className="form-group">
                <label htmlFor="cashDown">Cash Down *</label>
                <input
                  type="text"
                  id="cashDown"
                  name="cashDown"
                  value={formData.cashDown.toLocaleString('en-US', { style: 'currency', currency: 'USD' }) || '$0.00'}
                  onChange={handleChange}
                  required
                  placeholder="$0.00"
                />
              </div>
              <div className="form-group">
                <label>Amount Financed</label>
                <input
                  type="text"
                  value={`${financedAmount.toLocaleString('en-US', { style: 'currency', currency: 'USD' })}`}
                  disabled
                  className="calculated-field"
                />
              </div>
            </div>
            <div className="form-row">
              <div className="form-group">
                <label htmlFor="numberYears">Full Term Years *</label>
                <input
                  type="number"
                  id="numberYears"
                  name="numberYears"
                  value={formData.numberYears}
                  onChange={handleChange}
                  required
                  step="0.1"
                  min="0"
                />
              </div>
              <div className="form-group">
                <label htmlFor="frequency">Frequency / Period *</label>
                <select
                  id="frequency"
                  name="frequency"
                  value={formData.frequency}
                  onChange={handleChange}
                  required
                >
                  <option value="">Select Frequency</option>
                  {frequencies.map(freq => (
                    <option key={freq.autoId} value={freq.autoId}>
                      {freq.frequencyType} ({freq.paymentsPerYear}/year)
                    </option>
                  ))}
                </select>
              </div>
            </div>
            <div className="form-row">
              <div className="form-group">
                <label htmlFor="interestRate">Interest Rate (%) *</label>
                <input
                  type="text"
                  id="interestRate"
                  name="interestRate"
                  value={displayRate}
                  onChange={handleRateChange}
                  onBlur={handleRateBlur}
                  onFocus={() => setDisplayRate(displayRate.replace('%', ''))}
                  placeholder="0.0000%"
                />
              </div>
              <div className="form-group">
                <label htmlFor="periodInterestRate">Period Int. Rate</label>
                <input
                  type="number"
                  id="periodInterestRate"
                  name="periodInterestRate"
                  value={formData.periodInterestRate}
                  placeholder="0.0000000000"
                  disabled
                />
              </div>

            </div>
            <div className="form-row">
              <div className="form-group">
                <label htmlFor="correctionRate">Correction Rate (%)</label>
                <input
                  type="number"
                  id="correctionRate"
                  name="correctionRate"
                  value={formData.correctionRate}
                  onChange={handleChange}
                  step="0.0000000001"
                  placeholder="0.0000000000"
                />
              </div>
              <div className="form-group">
                <label htmlFor="finalPeriodRate">Final Period Rate</label>
                <input
                  type="number"
                  id="finalPeriodRate"
                  name="finalPeriodRate"
                  value={formData.finalPeriodRate}
                  onChange={handleChange}
                  step="0.000000001"
                  disabled
                  placeholder="0.0000000000"
                />
              </div>
            </div>
            <div className="form-row">
              <div className="form-group">
                <label htmlFor="annualTaxAmount">Annual Tax / Fees</label>
                <input
                  type="text"
                  id="annualTaxAmount"
                  name="annualTaxAmount"
                  value={formData.annualTaxAmount.toLocaleString('en-US', { style: 'currency', currency: 'USD' }) || '$0.00'}
                  onChange={AnnualTaxhandleChange}
                  placeholder="$0.00"
                />
              </div>
              <div className="form-group">
                <label htmlFor="annualTaxAmount">Period Tax / Fees</label>
                <input
                  type="text"
                  id="annualTaxAmount"
                  name="annualTaxAmount"
                  value={formData.periodTaxAmount.toLocaleString('en-US', { style: 'currency', currency: 'USD' }) || '$0.00'}
                  placeholder="$0.00"
                  disabled
                />
              </div>
            </div>
            <div className="form-section">
              <h3 className="section-title">
                Mortgage - Additional Payments
              </h3>
              {selectedFrequency && (
                <div className="result-item additional">
                  <span className="result-label additional">
                    {selectedFrequency.paymentsPerYear} additional payments per year will be application
                  </span>
                </div>
              )}
              <div className="form-row">
                <div className="form-group">
                  <label htmlFor="loanExtraPayment">Payment Amount</label>
                  <input
                    type="text"
                    id="loanExtraPayment"
                    name="loanExtraPayment"
                    value={formData.loanExtraPayment.toLocaleString('en-US', { style: 'currency', currency: 'USD' }) || '$0.00'}
                    onChange={handleChange}
                    placeholder="$0.00"
                  />
                </div>
                <div className="form-group">
                  <label htmlFor="newTermLength">New Length (Years)</label>
                  <input
                    type="number"
                    id="newTermLength"
                    name="newTermLength"
                    value={formData.newTermLength}
                    onChange={handleChange}
                    step="0.01"
                    min="0"
                    placeholder="0.00"
                    disabled
                  />
                </div>

              </div>
              <div className="form-row">
                <div className="form-group">
                  <label htmlFor="newNumberOfPayments">Num. Payments</label>
                  <input
                    type="number"
                    id="newNumberOfPayments"
                    name="newNumberOfPayments"
                    value={formData.newNumberOfPayments}
                    onChange={handleChange}
                    step="0.01"
                    min="0"
                    placeholder="0"
                    disabled
                  />
                </div>

                <div className="form-group">
                  <label htmlFor="extraPaymentTrigger">
                    <input
                      type="checkbox"
                      id="extraPaymentTrigger"
                      name="extraPaymentTrigger"
                      checked={formData.extraPaymentTrigger}
                      onChange={handleChange}
                    />
                    {' '}Check to Calculate using extra payment
                  </label>
                </div>
                {/* Notes Section */}
                <div className="form-section full-width">
                  <div className="form-group">
                    <label htmlFor="notes">Notes</label>
                    <textarea
                      id="notes"
                      name="notes"
                      value={formData.notes}
                      onChange={handleChange}
                      rows="4"
                      placeholder="Add any notes about this loan..."
                    />
                  </div>
                </div>
              </div>

            </div>
          </div>

          {/* Signed Term Information Section  style={{ marginTop: '0.5rem' }} */}
          <div className="form-section">
            <h3 className="section-title">Mortgage - Signed Term Information</h3>
            <div className="form-row">
              <div className="form-group">
                <label htmlFor="signedTermYears">Term Years</label>
                <input
                  type="number"
                  id="signedTermYears"
                  name="signedTermYears"
                  value={formData.signedTermYears}
                  onChange={handleChange}
                  step="0.1"
                  min="0"
                  placeholder="0"
                />
              </div>
              <div className="form-group">
                <label htmlFor="signedTermRemaining">Term Remaining</label>
                <input
                  type="number"
                  id="signedTermRemaining"
                  name="signedTermRemaining"
                  value={formData.signedTermRemaining}
                  onChange={handleChange}
                  placeholder="0"
                  disabled
                />
              </div>

            </div>

            <div className="form-row">

              <div className="form-group">
                <label htmlFor="signedTermType">Term Type</label>
                <select
                  id="signedTermType"
                  name="signedTermType"
                  value={formData.signedTermType}
                  onChange={handleChange}
                >
                  <option value="Fixed">Fixed</option>
                  <option value="Variable">Variable</option>
                </select>
              </div>
              <div className="form-group">
                <label htmlFor="signedTermNumberOfPayments">Term # Payment</label>
                <input
                  type="number"
                  id="signedTermNumberOfPayments"
                  name="signedTermNumberOfPayments"
                  value={formData.signedTermNumberOfPayments}
                  onChange={handleChange}
                  step="0.1"
                  min="0"
                  placeholder="0"
                  disabled
                />
              </div>
            </div>
            <div className="form-row">
              <div className="form-group">
                <label htmlFor="signedMaturityDate">Maturity Date</label>
                <input
                  type="date"
                  id="signedMaturityDate"
                  name="signedMaturityDate"
                  value={formData.signedMaturityDate}
                  onChange={handleChange}
                />
              </div>

            </div>

            {/* Results Section */}
            {loanDetails && (
              <div className="results-section">
                <h3 className="section-title results-title">Mortgage - Results</h3>
                {selectedFrequency && (
                  <div className="result-item">
                    <span className="result-label PaymentYear">
                      {selectedFrequency.paymentsPerYear} payments per year
                    </span>
                  </div>
                )}
                <div className="result-item">
                  <span className="result-label">Num. Payments:</span>
                  <span className="result-value">{loanDetails.numberOfPayments}</span>
                </div>
                <div className="result-item">
                  <span className="result-label">Payment Amount:</span>
                  <span className="result-value">
                    {Number(loanDetails.paymentAmount || 0).toLocaleString('en-US', { style: 'currency', currency: 'USD' })}
                  </span>
                </div>

                <div className="result-item">
                  <span className="result-label">Reg. Payment + Extra:</span>
                  <span className="result-value">
                    {Number(loanDetails.finalPaymentAmount || 0).toLocaleString('en-US', { style: 'currency', currency: 'USD' })}
                  </span>
                </div>

                <div className="result-item">
                  <span className="result-label">Including Tax / Fees:</span>
                  <span className="result-value">
                    {(
                      Number(loanDetails.finalPaymentAmount || 0) +
                      Number(loanDetails.monthlyTaxAmount || 0)
                    ).toLocaleString('en-US', { style: 'currency', currency: 'USD' })}
                  </span>
                </div>

                <div className="totals-section">
                  <div className="result-item">
                    <span className="result-label">Total Interest:</span>
                    <span className="result-value">
                      {loanDetails.totalInterest?.toLocaleString('en-US', { style: 'currency', currency: 'USD' }) || '0.00'}
                    </span>
                  </div>

                  <div className="result-item">
                    <span className="result-label">Total Principal:</span>
                    <span className="result-value">
                      {Number(loanDetails.totalPrincipal || 0).toLocaleString('en-US', { style: 'currency', currency: 'USD' })}
                    </span>
                  </div>

                  <div className="result-item">
                    <span className="result-label">Total Xtra Payment:</span>
                    <span className="result-value">
                      {Number(loanDetails.totalExtraPayment || 0).toLocaleString('en-US', { style: 'currency', currency: 'USD' })}
                    </span>
                  </div>

                  <div className="result-item">
                    <span className="result-label">Total Princ. + Xtra:</span>
                    <span className="result-value">
                      {(
                        Number(loanDetails.totalPrincipal || 0) +
                        Number(loanDetails.totalExtraPayment || 0)
                      ).toLocaleString('en-US', { style: 'currency', currency: 'USD' })}
                    </span>
                  </div>

                  <div className="result-item grand-total">
                    <span className="result-label">Total Paid (All):</span>
                    <span className="result-value">
                      {(
                        Number(loanDetails.totalPrincipal || 0) +
                        Number(loanDetails.totalInterest || 0) +
                        Number(loanDetails.totalExtraPayment || 0)
                      ).toLocaleString('en-US', { style: 'currency', currency: 'USD' })}
                    </span>
                  </div>

                  <div className="result-item grandblack-total">
                    <span className="result-label">Grand Total Paid:</span>
                    <span className="result-value">
                      {(
                        Number(loanDetails.totalPrincipal || 0) +
                        Number(loanDetails.totalInterest || 0) +
                        Number(loanDetails.totalExtraPayment || 0) +
                        Number(parseCurrency(formData.cashDown) || 0)
                      ).toLocaleString('en-US', { style: 'currency', currency: 'USD' })}
                    </span>
                  </div>
                </div>
              </div>
            )}

          </div>
        </div>



        {/* Action Buttons */}
        <div className="form-actions">
          <button
            type="button"
            className="btn-cancel"
            onClick={() => navigate('/')}
            disabled={loading}
          >
            <FaTimes /> Cancel
          </button>

          <button
            type="submit"
            className="btn-save"
            disabled={loading}
          >
            <FaSave /> {loading ? 'Saving...' : 'Save Loan'}
          </button>

          {isEditMode && (
            <>
              <button
                type="button"
                className="btn-calculate"
                onClick={handleCalculate}
                disabled={loading}
              >
                <FaCalculator /> Calculate Loan
              </button>

              <button
                type="button"
                className="btn-schedule"
                onClick={handleGenerateSchedule}
                disabled={loading}
              >
                <FaListAlt /> Create Schedule
              </button>

              <button
                type="button"
                className="btn-toggle"
                onClick={handleToggleSchedule}
              >
                <FaListAlt /> {showSchedule ? 'Hide' : 'Show'} Schedule
              </button>

              <button
                type="button"
                className="btn-toggle"
                onClick={handleToggleChart}
              >
                <FaChartArea /> {showChart ? 'Hide' : 'Show'} Graph
              </button>

              <button
                type="button"
                className="btn-upload"
                onClick={() => {
                  setSelectedLoanId(loanDetails?.autoId || id);
                  setShowPopup(true);
                }}
              >
                📎 Upload Documents
              </button>
              {showPopup && (
                <AttachmentPopup
                  loanId={selectedLoanId}
                  onClose={() => setShowPopup(false)}
                  onUpload={() => fetchAttachments(selectedLoanId)} // optional refresh
                />
              )}
            </>
          )}
        </div>
      </form>

      {/* Payment Schedule Table */}
      {isEditMode && showSchedule && (
        <div className="schedule-section">
          <PaymentScheduleTable loanId={id} schedule={schedule} />
        </div>
      )}

      {/* Loan Chart */}
      {isEditMode && showChart && (
        <div className="chart-section">
          <LoanChart loanId={id} />
        </div>
      )}
    </div>
  );
}

export default LoanForm;
