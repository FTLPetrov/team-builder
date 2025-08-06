import { Link } from 'react-router-dom';
import Button from '../../components/Button';

const Unauthorized = () => {
  return (
    <div className="min-h-screen bg-gray-50 flex items-center justify-center px-4">
      <div className="max-w-md w-full text-center">
        {/* 401 Icon */}
        <div className="mb-8">
          <div className="mx-auto w-24 h-24 bg-yellow-100 rounded-full flex items-center justify-center">
            <span className="text-4xl font-bold text-yellow-600">401</span>
          </div>
        </div>

        {/* Error Message */}
        <h1 className="text-3xl font-bold text-gray-900 mb-4">
          Access Denied
        </h1>
        <p className="text-gray-600 mb-8">
          You need to be logged in to access this page. Please sign in to continue.
        </p>

        {/* Action Buttons */}
        <div className="space-y-4">
          <Link to="/login">
            <Button className="w-full">
              Sign In
            </Button>
          </Link>
          <Link to="/register">
            <Button
              variant="outline"
              className="w-full"
            >
              Create Account
            </Button>
          </Link>
        </div>

        {/* Additional Help */}
        <div className="mt-8 pt-8 border-t border-gray-200">
          <p className="text-sm text-gray-500 mb-2">
            Don't have an account? Create one for free
          </p>
          <Link 
            to="/" 
            className="text-blue-600 hover:text-blue-500 text-sm font-medium"
          >
            Learn More
          </Link>
        </div>
      </div>
    </div>
  );
};

export default Unauthorized;
