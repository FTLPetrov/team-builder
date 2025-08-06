import { useState, useEffect } from 'react';
import { invitationService } from '../services/api/invitationService';

export const useInvitations = () => {
  const [invitations, setInvitations] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const fetchInvitations = async () => {
    try {
      setLoading(true);
      const data = await invitationService.getUserInvitations();
      setInvitations(data);
      setError(null);
    } catch (err) {
      setError(err.message);
      console.error('Error fetching invitations:', err);
    } finally {
      setLoading(false);
    }
  };

  const respondToInvitation = async (invitationId, accept) => {
    try {
      const result = await invitationService.respondToInvitation(invitationId, accept ? 'accept' : 'decline');
      

      setInvitations(prev => prev.filter(inv => inv.id !== invitationId));
      

      window.dispatchEvent(new CustomEvent('invitationCountChanged'));
      
      return { success: true, data: result };
    } catch (err) {
      setError(err.message);
      return { success: false, error: err.message };
    }
  };

  useEffect(() => {
    fetchInvitations();
  }, []);

  useEffect(() => {
    const handleInvitationCountChanged = () => {
      fetchInvitations();
    };

    window.addEventListener('invitationCountChanged', handleInvitationCountChanged);
    return () => {
      window.removeEventListener('invitationCountChanged', handleInvitationCountChanged);
    };
  }, []);

  return {
    invitations,
    loading,
    error,
    fetchInvitations,
    respondToInvitation
  };
}; 