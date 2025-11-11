import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { frequencyAPI } from '../services/api';
import { FaSave, FaTimes, FaStepBackward, FaBackward, FaForward, FaStepForward, FaPlus, FaLock, FaLockOpen, FaTrash, FaSearch, FaSearchPlus, FaSearchMinus } from 'react-icons/fa';
import './FrequencyForm.css';

function FrequencyForm() {
  const [formData, setFormData] = useState({ 
    frequencyType: '', 
    paymentsPerYear: '', 
    daysBetweenPayments: '' 
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [message, setMessage] = useState(null);
  const [isLocked, setIsLocked] = useState(true);
  const [totalCount, setTotalCount] = useState(0);
  const navigate = useNavigate();
  const { id } = useParams();

  useEffect(() => {
    if (id) {
      fetchFrequency();
    }
    fetchCount();
  }, [id]);

  const fetchFrequency = async () => {
    try {
      const response = await frequencyAPI.getById(id);
      setFormData(response.data);
    } catch (err) {
      setError('Failed to load frequency');
      console.error('Error:', err);
    }
  };

  const fetchCount = async () => {
    try {
      const response = await frequencyAPI.getAll();
      setTotalCount(response.data.length);
    } catch (err) {
      console.error('Error fetching count:', err);
    }
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    if (!isLocked || !id) {
      setFormData({ ...formData, [name]: value });
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (isLocked && id) {
      setError('Form is locked. Click unlock to edit.');
      return;
    }

    setLoading(true);
    setError(null);
    setMessage(null);

    try {
      if (id) {
        await frequencyAPI.update(id, formData);
        setMessage('Frequency updated successfully!');
        setIsLocked(true);
      } else {
        const response = await frequencyAPI.create(formData);
        setMessage('Frequency created successfully!');
        navigate(`/frequencies/edit/${response.data.autoId}`);
      }
      fetchCount();
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to save frequency');
    } finally {
      setLoading(false);
    }
  };

  const handleNavigation = async (action) => {
    if (!id) return;

    try {
      let response;
      switch (action) {
        case 'first':
          response = await frequencyAPI.getAll();
          if (response.data.length > 0) {
            navigate(`/frequencies/edit/${response.data[0].autoId}`);
          }
          break;
        case 'previous':
          response = await frequencyAPI.getAll();
          const currentIndex = response.data.findIndex(f => f.autoId === parseInt(id));
          if (currentIndex > 0) {
            navigate(`/frequencies/edit/${response.data[currentIndex - 1].autoId}`);
          }
          break;
        case 'next':
          response = await frequencyAPI.getAll();
          const nextIndex = response.data.findIndex(f => f.autoId === parseInt(id));
          if (nextIndex < response.data.length - 1) {
            navigate(`/frequencies/edit/${response.data[nextIndex + 1].autoId}`);
          }
          break;
        case 'last':
          response = await frequencyAPI.getAll();
          if (response.data.length > 0) {
            navigate(`/frequencies/edit/${response.data[response.data.length - 1].autoId}`);
          }
          break;
        case 'new':
          navigate('/frequencies/new');
          break;
      }
    } catch (err) {
      console.error('Navigation error:', err);
    }
  };

  const handleRefresh = async () => {
    if (id) {
      await fetchFrequency();
      setMessage('Data refreshed successfully');
      setTimeout(() => setMessage(null), 2000);
    }
  };

  const handleUnlock = () => {
    if (window.confirm('Do you wish to unlock this form for edits?')) {
      setIsLocked(false);
      setMessage('Form unlocked for editing');
    }
  };

  const handleLock = () => {
    setIsLocked(true);
    setMessage('Form locked');
  };

  const handleDelete = async () => {
    if (!id) return;
    
    if (window.confirm('Are you sure you want to delete this frequency?')) {
      try {
        await frequencyAPI.delete(id);
        setMessage('Frequency deleted successfully');
        navigate('/frequencies');
      } catch (err) {
        setError(err.response?.data?.message || 'Failed to delete frequency');
      }
    }
  };

  const getCurrentPosition = () => {
    if (!id) return 'New Record';
    return `Record ${id} of ${totalCount}`;
  };

  return (
    <div className="frequency-form-container">
      <div className="form-header">
        <h2>Insert / Edit</h2>
      </div>

      {/* Navigation Toolbar */}
      <div className="navigation-toolbar">
        <button 
          type="button" 
          className="nav-btn" 
          onClick={() => handleNavigation('first')}
          disabled={!id}
          title="First Record"
        >
          <FaStepBackward />
        </button>
        
        <button 
          type="button" 
          className="nav-btn" 
          onClick={() => handleNavigation('previous')}
          disabled={!id}
          title="Previous Record"
        >
          <FaBackward />
        </button>

        <button 
          type="button" 
          className="nav-btn" 
          onClick={handleRefresh}
          disabled={!id}
          title="Refresh"
        >
          <FaSave />
        </button>

        <button 
          type="button" 
          className="nav-btn" 
          onClick={() => handleNavigation('new')}
          title="New Record"
        >
          <FaPlus />
        </button>

        <button 
          type="button" 
          className="nav-btn" 
          onClick={handleDelete}
          disabled={!id}
          title="Delete Record"
        >
          <FaTrash />
        </button>

        <button 
          type="button" 
          className="nav-btn" 
          onClick={isLocked ? handleUnlock : handleLock}
          disabled={!id}
          title={isLocked ? "Unlock Form" : "Lock Form"}
        >
          {isLocked ? <FaLock /> : <FaLockOpen />}
        </button>

        <button 
          type="button" 
          className="nav-btn" 
          title="Search"
        >
          <FaSearch />
        </button>

        <button 
          type="button" 
          className="nav-btn" 
          title="Zoom In"
        >
          <FaSearchPlus />
        </button>

        <button 
          type="button" 
          className="nav-btn" 
          title="Zoom Out"
        >
          <FaSearchMinus />
        </button>

        <button 
          type="button" 
          className="nav-btn" 
          onClick={() => handleNavigation('next')}
          disabled={!id}
          title="Next Record"
        >
          <FaForward />
        </button>

        <button 
          type="button" 
          className="nav-btn" 
          onClick={() => handleNavigation('last')}
          disabled={!id}
          title="Last Record"
        >
          <FaStepForward />
        </button>
      </div>

      {error && <div className="error-message">{error}</div>}
      {message && <div className="success-message">{message}</div>}

      <form onSubmit={handleSubmit} className="frequency-form">
        <div className="form-group">
          <label htmlFor="frequencyType">Description</label>
          <input
            type="text"
            id="frequencyType"
            name="frequencyType"
            value={formData.frequencyType}
            onChange={handleChange}
            required
            disabled={isLocked && id}
            className={isLocked && id ? 'locked' : ''}
          />
        </div>

        <div className="form-group">
          <label htmlFor="paymentsPerYear">Payment per year</label>
          <input
            type="number"
            id="paymentsPerYear"
            name="paymentsPerYear"
            value={formData.paymentsPerYear}
            onChange={handleChange}
            required
            disabled={isLocked && id}
            className={isLocked && id ? 'locked' : ''}
          />
        </div>

        <div className="form-group">
          <label htmlFor="daysBetweenPayments">Days Between Payments</label>
          <input
            type="number"
            id="daysBetweenPayments"
            name="daysBetweenPayments"
            value={formData.daysBetweenPayments}
            onChange={handleChange}
            required
            disabled={isLocked && id}
            className={isLocked && id ? 'locked' : ''}
          />
        </div>

        <div className="status-bar">
          <span className="record-position">{getCurrentPosition()}</span>
          <span className={`lock-status ${isLocked ? 'locked' : 'unlocked'}`}>
            Status: {isLocked ? 'LOCKED' : 'UNLOCKED'}
          </span>
        </div>

        <div className="form-actions">
          <button 
            type="button" 
            className="btn-cancel"
            onClick={() => navigate('/frequencies')}
          >
            <FaTimes /> Cancel
          </button>
          <button 
            type="submit" 
            className="btn-save"
            disabled={loading || (isLocked && id)}
          >
            <FaSave /> {loading ? 'Saving...' : 'Save'}
          </button>
        </div>
      </form>
    </div>
  );
}

export default FrequencyForm;
