import Home from '../pages/Home';
import Login from '../pages/Login';
import Register from '../pages/Register';
import Dashboard from '../pages/Dashboard';
import Teams from '../pages/Teams';
import Events from '../pages/Events';
import Invitations from '../pages/Invitations';
import CreateTeam from '../pages/CreateTeam';
import TeamDetail from '../pages/TeamDetail';
import Profile from '../pages/Profile';

export const routes = [
  {
    path: '/',
    element: Home,
    public: true,
    title: 'Home'
  },
  {
    path: '/login',
    element: Login,
    public: true,
    title: 'Login'
  },
  {
    path: '/register',
    element: Register,
    public: true,
    title: 'Register'
  },
  {
    path: '/dashboard',
    element: Dashboard,
    protected: true,
    title: 'Dashboard'
  },
  {
    path: '/teams',
    element: Teams,
    protected: true,
    title: 'Teams'
  },
  {
    path: '/teams/create',
    element: CreateTeam,
    protected: true,
    title: 'Create Team'
  },
  {
    path: '/teams/:teamId',
    element: TeamDetail,
    protected: true,
    title: 'Team Details'
  },
  {
    path: '/events',
    element: Events,
    protected: true,
    title: 'Events'
  },
  {
    path: '/invitations',
    element: Invitations,
    protected: true,
    title: 'Invitations'
  },
  {
    path: '/profile',
    element: Profile,
    protected: true,
    title: 'Profile'
  }
];

export const getPublicRoutes = () => routes.filter(route => route.public);
export const getProtectedRoutes = () => routes.filter(route => route.protected);
export const getAllRoutes = () => routes; 