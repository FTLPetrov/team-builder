import { Routes, Route } from 'react-router-dom';
import { getAllRoutes } from './routes';
import ProtectedRoute from '../../components/auth/ProtectedRoute';
import AdminRoute from '../../components/auth/AdminRoute';
import NotFound from '../../pages/errors/NotFound';

const AppRoutes = () => {
  const routes = getAllRoutes();

  return (
    <Routes>
      {routes.map(({ path, element: Element, protected: isProtected, admin: isAdmin }) => (
        <Route
          key={path}
          path={path}
          element={
            isAdmin ? (
              <AdminRoute>
                <Element />
              </AdminRoute>
            ) : isProtected ? (
              <ProtectedRoute>
                <Element />
              </ProtectedRoute>
            ) : (
              <Element />
            )
          }
        />
      ))}
      <Route path="*" element={<NotFound />} />
    </Routes>
  );
};

export default AppRoutes; 