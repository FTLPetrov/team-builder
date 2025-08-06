import { Link } from 'react-router-dom';
import Button from '../../components/Button';

const ServerError = () => {
  return (
    <div className="min-h-screen bg-gray-50 flex items-center justify-center px-4">
      <div className="max-w-md w-full text-center">
        {/* 500 Icon */}
        <div className="mb-8">
          <div className="mx-auto w-24 h-24 bg-orange-100 rounded-full flex items-center justify-center">
            <span className="text-4xl font-bold text-orange-600">500</span>
          </div>
        </div>

        {/* Error Message */}
        <h1 className="text-3xl font-bold text-gray-900 mb-4">
          Server Error
        </h1>
        <p className="text-gray-600 mb-8">
          Something went wrong on our end. We're working to fix this issue.
        </p>

        {/* Action Buttons */}
        <div className="space-y-4">
          <Button
            onClick={() => window.location.reload()}
            className="w-full"
          >
            Try Again
          </Button>
          <Link to="/">
            <Button
              variant="outline"
              className="w-full"
            >
              Go to Home
            </Button>
          </Link>
        </div>

        {/* Additional Help */}
        <div className="mt-8 pt-8 border-t border-gray-200">
          <p className="text-sm text-gray-500 mb-2">
            If this problem persists, please contact our support team
          </p>
          <Link 
            to="/contact" 
            className="text-blue-600 hover:text-blue-500 text-sm font-medium"
          >
            Contact Support
          </Link>
        </div>
      </div>
    </div>
  );
};

export default ServerError;
