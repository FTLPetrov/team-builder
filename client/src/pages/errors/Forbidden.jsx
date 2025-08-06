import { Link } from 'react-router-dom';
import Button from '../../components/Button';

const Forbidden = () => {
  return (
    <div className="min-h-screen bg-gray-50 flex items-center justify-center px-4">
      <div className="max-w-md w-full text-center">
        {/* 403 Icon */}
        <div className="mb-8">
          <div className="mx-auto w-24 h-24 bg-purple-100 rounded-full flex items-center justify-center">
            <span className="text-4xl font-bold text-purple-600">403</span>
          </div>
        </div>

        {/* Error Message */}
        <h1 className="text-3xl font-bold text-gray-900 mb-4">
          Access Forbidden
        </h1>
        <p className="text-gray-600 mb-8">
          You don't have permission to access this page. This area is restricted to administrators only.
        </p>

        {/* Action Buttons */}
        <div className="space-y-4">
          <Link to="/dashboard">
            <Button className="w-full">
              Go to Dashboard
            </Button>
          </Link>
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
            Need admin access? Contact your system administrator
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

export default Forbidden;
