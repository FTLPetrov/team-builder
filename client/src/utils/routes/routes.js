import Home from '../../pages/Home';
import Login from '../../pages/Login';
import Register from '../../pages/Register';
import Dashboard from '../../pages/Dashboard';
import Teams from '../../pages/Teams';
import Events from '../../pages/Events';
import CreateTeam from '../../pages/CreateTeam';
import CreateEvent from '../../pages/CreateEvent';
import TeamDetail from '../../pages/TeamDetail';
import EventDetail from '../../pages/EventDetail';
import Profile from '../../pages/Profile';
import AboutUs from '../../pages/AboutUs';
import ContactUs from '../../pages/ContactUs';
import Chat from '../../pages/Chat';
import AdminDashboard from '../../pages/AdminDashboard';
import AdminUsers from '../../pages/AdminUsers';
import AdminTeams from '../../pages/AdminTeams';
import AdminEvents from '../../pages/AdminEvents';
import AdminSupport from '../../pages/AdminSupport';
import AdminNotices from '../../pages/AdminNotices';
import NotFound from '../../pages/errors/NotFound';
import ServerError from '../../pages/errors/ServerError';
import Unauthorized from '../../pages/errors/Unauthorized';
import Forbidden from '../../pages/errors/Forbidden';

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
    path: '/about',
    element: AboutUs,
    protected: true,
    title: 'About Us'
  },
  {
    path: '/contact',
    element: ContactUs,
    protected: true,
    title: 'Contact Support'
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
    path: '/teams/:teamId/chat',
    element: Chat,
    protected: true,
    title: 'Team Chat'
  },
  {
    path: '/events',
    element: Events,
    protected: true,
    title: 'Events'
  },
  {
    path: '/events/create',
    element: CreateEvent,
    protected: true,
    title: 'Create Event'
  },
  {
    path: '/events/:eventId',
    element: EventDetail,
    protected: true,
    title: 'Event Details'
  },
  {
    path: '/profile',
    element: Profile,
    protected: true,
    title: 'Profile'
  },

  {
    path: '/admin',
    element: AdminDashboard,
    protected: true,
    admin: true,
    title: 'Admin Dashboard'
  },
  {
    path: '/admin/users',
    element: AdminUsers,
    protected: true,
    admin: true,
    title: 'Admin Users'
  },
  {
    path: '/admin/teams',
    element: AdminTeams,
    protected: true,
    admin: true,
    title: 'Admin Teams'
  },
  {
    path: '/admin/events',
    element: AdminEvents,
    protected: true,
    admin: true,
    title: 'Admin Events'
  },
  {
    path: '/admin/support',
    element: AdminSupport,
    protected: true,
    admin: true,
    title: 'Admin Support'
  },
  {
    path: '/admin/notices',
    element: AdminNotices,
    protected: true,
    admin: true,
    title: 'Admin Notices'
  },

  {
    path: '/error/500',
    element: ServerError,
    public: true,
    title: 'Server Error'
  },
  {
    path: '/error/401',
    element: Unauthorized,
    public: true,
    title: 'Unauthorized'
  },
  {
    path: '/error/403',
    element: Forbidden,
    public: true,
    title: 'Forbidden'
  }
];

export const getPublicRoutes = () => routes.filter(route => route.public);
export const getProtectedRoutes = () => routes.filter(route => route.protected);
export const getAllRoutes = () => routes; 