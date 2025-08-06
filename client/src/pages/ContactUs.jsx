import { useState } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { supportService } from '../services/api/supportService';
import Button from '../components/Button';
import Card, { CardHeader, CardTitle, CardContent } from '../components/Card';

const ContactUs = () => {
  const { isAuthenticated, user } = useAuth();
  const [formData, setFormData] = useState({
    subject: '',
    message: ''
  });
  const [submitting, setSubmitting] = useState(false);
  const [submitted, setSubmitted] = useState(false);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    if (!isAuthenticated) {
      alert('Please log in to submit a support request.');
      return;
    }
    
    if (!formData.subject.trim() || !formData.message.trim()) {
      alert('Please fill in both subject and message fields.');
      return;
    }
    
    setSubmitting(true);
    
    try {
      const result = await supportService.createSupportMessage({
        subject: formData.subject,
        message: formData.message
      });
      
      setSubmitted(true);
      setFormData({ subject: '', message: '' });
    } catch (error) {
      console.error('Error submitting support request:', error);
      if (error.response?.status === 401) {
        alert('Authentication failed. Please log in again.');
      } else {
        alert('Failed to submit support request. Please try again.');
      }
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
        {/* Header */}
        <div className="text-center mb-12">
          <h1 className="text-4xl font-bold text-gray-900 mb-4">Contact Support</h1>
          <p className="text-xl text-gray-600 max-w-2xl mx-auto">
            Need help with TeamBuilder? Submit a support request and we'll assist you as soon as possible.
          </p>
        </div>

        <div className="max-w-2xl mx-auto">
          {/* Contact Form */}
          <Card>
            <CardHeader>
              <CardTitle>Submit Support Request</CardTitle>
            </CardHeader>
            <CardContent>
              {submitted ? (
                <div className="text-center py-8">
                  <div className="w-16 h-16 bg-green-100 rounded-full flex items-center justify-center mx-auto mb-4">
                    <svg className="w-8 h-8 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                    </svg>
                  </div>
                  <h3 className="text-lg font-medium text-gray-900 mb-2">Support Request Submitted!</h3>
                  <p className="text-gray-600 mb-4">Thank you for contacting support. We'll get back to you soon.</p>
                  <Button onClick={() => setSubmitted(false)} variant="outline">
                    Submit Another Request
                  </Button>
                </div>
              ) : (
                <form onSubmit={handleSubmit} className="space-y-6">
                  {!isAuthenticated && (
                    <div className="bg-yellow-50 border border-yellow-200 rounded-md p-4">
                      <div className="flex items-center justify-between">
                        <p className="text-yellow-800 text-sm">
                          Please log in to submit a support request.
                        </p>
                        <Button 
                          onClick={() => window.location.href = '/login'} 
                          size="small"
                          variant="outline"
                        >
                          Log In
                        </Button>
                      </div>
                    </div>
                  )}
                  
                  <div>
                    <label htmlFor="subject" className="block text-sm font-medium text-gray-700 mb-2">
                      Subject
                    </label>
                    <input
                      type="text"
                      id="subject"
                      name="subject"
                      value={formData.subject}
                      onChange={handleChange}
                      required
                      disabled={!isAuthenticated}
                      className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 disabled:bg-gray-100 disabled:cursor-not-allowed"
                    />
                  </div>

                  <div>
                    <label htmlFor="message" className="block text-sm font-medium text-gray-700 mb-2">
                      Message
                    </label>
                    <textarea
                      id="message"
                      name="message"
                      rows={6}
                      value={formData.message}
                      onChange={handleChange}
                      required
                      disabled={!isAuthenticated}
                      className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 disabled:bg-gray-100 disabled:cursor-not-allowed"
                    />
                  </div>

                  <Button
                    type="submit"
                    disabled={submitting || !isAuthenticated}
                    className="w-full"
                  >
                    {submitting ? 'Submitting...' : 'Submit Support Request'}
                  </Button>
                </form>
              )}
            </CardContent>
          </Card>

          {/* Support Hours */}
          <Card className="mt-8">
            <CardHeader>
              <CardTitle>Support Hours</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="space-y-3">
                <div className="flex justify-between">
                  <span className="text-gray-600">Monday - Friday</span>
                  <span className="font-medium">9:00 AM - 6:00 PM EST</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-gray-600">Saturday</span>
                  <span className="font-medium">10:00 AM - 4:00 PM EST</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-gray-600">Sunday</span>
                  <span className="font-medium">Closed</span>
                </div>
              </div>
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  );
};

export default ContactUs; 