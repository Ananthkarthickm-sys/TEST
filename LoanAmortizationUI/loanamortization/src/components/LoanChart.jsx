import React, { useState, useEffect } from 'react';
import { LineChart, Line, AreaChart, Area, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';
import { paymentScheduleAPI } from '../services/api';
import './LoanChart.css';

function LoanChart({ loanId }) {
  const [chartData, setChartData] = useState({ principalData: [], interestData: [] });
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchChartData();
  }, [loanId]);

  const fetchChartData = async () => {
    try {
      const response = await paymentScheduleAPI.getChartData(loanId);
      setChartData(response.data);
    } catch (err) {
      console.error('Error fetching chart data:', err);
    } finally {
      setLoading(false);
    }
  };

  if (loading) return <div>Loading chart...</div>;

  const combinedData = chartData.principalData.map((p, idx) => ({
    paymentNumber: p.paymentNumber,
    principal: p.value,
    interest: chartData.interestData[idx]?.value || 0
  }));

  return (
    <div className="loan-chart-container">
      <h3>Interest versus principal comparison</h3>
      <ResponsiveContainer width="100%" height={400}>
        <AreaChart data={combinedData} margin={{ top: 10, right: 30, left: 0, bottom: 0 }}>
          <CartesianGrid strokeDasharray="3 3" />
          <XAxis dataKey="paymentNumber" label={{ value: 'Payment number', position: 'insideBottom', offset: -5 }} />
          <YAxis label={{ value: 'Interest and Principal relationship', angle: -90, position: 'insideLeft' }} />
          <Tooltip formatter={(value) => `$${value.toFixed(2)}`} />
          <Legend />
          <Area type="monotone" dataKey="principal" stackId="1" stroke="#d88484" fill="#d88484" name="Principal" />
          <Area type="monotone" dataKey="interest" stackId="2" stroke="#2563eb" fill="transparent" name="Interest" />
        </AreaChart>
      </ResponsiveContainer>
    </div>
  );
}

export default LoanChart;
