import React, { useState, useEffect } from 'react';
import { loanAPI, frequencyAPI } from '../services/api';
import './LoanCalculator.css';

function LoanCalculator() {
  const [frequencies, setFrequencies] = useState([]);
  const [formData, setFormData] = useState({
    purchasePrice: '', cashDown: '', frequencyId: '', numberYears: '',
    interestRate: '', correctionRate: '0', loanExtraPayment: '0',
    annualTaxAmount: '0', extraPaymentTrigger: false
  });
  const [result, setResult] = useState(null);
 const [displayRate, setDisplayRate] = useState(() => {
  // Initialize display value based on existing formData
  return formData.interestRate
    ? `${(formData.interestRate * 100).toFixed(4)}%`
    : '';
});
  useEffect(() => {
    fetchFrequencies();
  }, []);
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
const parsePercentage = (value) => {
  if (value === null || value === undefined) return 0;
  const cleaned = value.toString().replace(/[^0-9.-]/g, '');
  const parsed = parseFloat(cleaned);
  return isNaN(parsed) ? 0 : parsed; // 70000 -> 0.07
};
const handleRateBlur = () => {
  const numeric = parsePercentage(displayRate); // e.g. 70000 -> 0.07
  setFormData(prev => ({ ...prev, interestRate: numeric }));

  // Show formatted percentage (7.0000%)
  setDisplayRate(`${(numeric).toFixed(4)}%`);
};

  const fetchFrequencies = async () => {
    const response = await frequencyAPI.getAll();
    setFrequencies(response.data);
  };

  const handleCalculate = async () => {
    try {
      const response = await loanAPI.calculate({
        ...formData,
        purchasePrice: parseFloat(formData.purchasePrice),
        cashDown: parseFloat(formData.cashDown),
        frequencyId: parseInt(formData.frequencyId),
        numberYears: parseFloat(formData.numberYears),
        interestRate: parseFloat(formData.interestRate),
        correctionRate: parseFloat(formData.correctionRate),
        loanExtraPayment: parseFloat(formData.loanExtraPayment),
        annualTaxAmount: parseFloat(formData.annualTaxAmount)
      });
      setResult(response.data);
    } catch (err) {
      alert('Calculation failed');
    }
  };

  return (
    <div className="calculator-container">
      <h2>Loan Calculator</h2>
      <div className="calculator-form">
        <input type="number" placeholder="Purchase Price" value={formData.purchasePrice} onChange={(e) => setFormData({...formData, purchasePrice: e.target.value})} />
        <input type="number" placeholder="Cash Down" value={formData.cashDown} onChange={(e) => setFormData({...formData, cashDown: e.target.value})} />
        <select value={formData.frequencyId} onChange={(e) => setFormData({...formData, frequencyId: e.target.value})}>
          <option value="">Select Frequency</option>
          {frequencies.map(f => <option key={f.autoId} value={f.autoId}>{f.frequencyType}</option>)}
        </select>
        <input type="number" placeholder="Years" value={formData.numberYears} onChange={(e) => setFormData({...formData, numberYears: e.target.value})} />
     
        <input type="text"
              id="interestRate"
              name="interestRate"
              value={displayRate}
              onChange={handleRateChange}
              onBlur={handleRateBlur}
              onFocus={() => setDisplayRate(displayRate.replace('%', ''))}
              placeholder="0.0000%"
         />
        <button onClick={handleCalculate}>Calculate</button>
      </div>
      {result && (
        <div className="calculator-results">
          <h3>Results</h3>
          <p>Financed Amount: {result.financedAmount?.toLocaleString('en-US', { style: 'currency', currency: 'USD' })}</p>
          <p>Number of Payments: {result.numberOfPayments}</p>
          <p>Payment Amount: {result.paymentAmount?.toLocaleString('en-US', { style: 'currency', currency: 'USD' })}</p>
          <p>Total with Tax: {result.totalPaymentWithTax?.toLocaleString('en-US', { style: 'currency', currency: 'USD' })}</p>
          <p>{result.message}</p>
        </div>
      )}
    </div>
  );
}

export default LoanCalculator;
