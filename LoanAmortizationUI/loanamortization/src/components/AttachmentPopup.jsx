import React, { useState, useEffect } from "react";
import axios from "axios";
import "./AttachmentPopup.css";

const API_BASE_URL = 'https://localhost:55546/api/Attachments';

export default function AttachmentPopup({ loanId, onClose }) {
    const [file, setFile] = useState(null);
    const [attachments, setAttachments] = useState([]);
    const [uploading, setUploading] = useState(false);
    const [deletingId, setDeletingId] = useState(null); // track which file is being deleted

    useEffect(() => {
        fetchAttachments();
    }, [loanId]);

    const fetchAttachments = async () => {
        try {
            const res = await axios.get(`${API_BASE_URL}/GetByLoan/${loanId}`);
            setAttachments(res.data);
        } catch (err) {
            console.error("Error fetching attachments:", err);
        }
    };

    const handleUpload = async (e) => {
        e.preventDefault();
        e.stopPropagation();
        if (!file) return alert("Please choose a file first");

        const formData = new FormData();
        formData.append("file", file);
        formData.append("loanId", loanId);

        setUploading(true);
        try {
            const response = await axios.post(`${API_BASE_URL}/upload`, formData, {
                headers: { "Content-Type": "multipart/form-data" },
            });
            console.log("Uploaded:", response.data.attachmentId);
            setFile(null);
            fetchAttachments();
        } catch (err) {
            alert("Upload failed");
        } finally {
            setUploading(false);
        }
    };

    const handleDelete = async (id) => {
        if (!window.confirm("Are you sure you want to delete this file?")) return;

        setDeletingId(id);
        try {
            await axios.delete(`${API_BASE_URL}/Delete/${id}`);
            fetchAttachments(); // refresh list after deletion
        } catch (err) {
            alert("Delete failed");
            console.error(err);
        } finally {
            setDeletingId(null);
        }
    };

    return (
        <div className="popup-overlay attachment-popup">
            <div className="popup-container">
                <h3>Upload Documents for Loan #{loanId}</h3>

                <input
                    type="file"
                    onChange={(e) => setFile(e.target.files[0])}
                    disabled={uploading}
                />


                <h4>Uploaded Files</h4>
                <ul className="file-list">
                    {attachments.length === 0 ? (
                        <li>No documents uploaded yet</li>
                    ) : (
                        attachments.map((a) => (
                            <li key={a.attachmentId}>
                                <a
                                    href={`${API_BASE_URL}/download/${a.attachmentId}`}
                                    target="_blank"
                                    rel="noreferrer"
                                    title={a.fileName}
                                >
                                    {a.fileName}
                                </a>
                                <button
                                    className="btn-delete"
                                    onClick={() => handleDelete(a.attachmentId)}
                                    disabled={deletingId === a.attachmentId}
                                >
                                    {deletingId === a.attachmentId ? "X..." : "X"}
                                </button>
                            </li>
                        ))
                    )}
                </ul>
                {/* Flex container for Upload and Close buttons */}
                <div className="button-row">
                    <button type="submit" onClick={handleUpload} disabled={uploading}>
                        {uploading ? "Uploading..." : "Upload"}
                    </button>
                    <button className="btn-close" onClick={onClose}>
                        Close
                    </button>
                </div>
            </div>
        </div>
    );

}
