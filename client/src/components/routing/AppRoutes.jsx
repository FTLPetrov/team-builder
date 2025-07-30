import { Routes, Route, Navigate } from 'react-router-dom';
import { getAllRoutes } from '../../config/routes';
import ProtectedRoute from '../auth/ProtectedRoute';

const AppRoutes = () => {
  const routes = getAllRoutes();

  return (
    <Routes>
      {routes.map(({ path, element: Element, protected: isProtected }) => (
        <Route
          key={path}
          path={path}
          element={
            isProtected ? (
              <ProtectedRoute>
                <Element />
              </ProtectedRoute>
            ) : (
              <Element />
            )
          }
        />
      ))}
      <Route path="*" element={<Navigate to="/" />} />
    </Routes>
  );
};

export default AppRoutes; 