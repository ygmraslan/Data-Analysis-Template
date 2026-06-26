import { createBrowserRouter } from 'react-router-dom'
import Login from '../pages/auth/Login'
import Mfa from '../pages/auth/Mfa'
import MfaSetup from '../pages/auth/MfaSetup'
import ForgotPassword from '../pages/auth/ForgotPassword'
import ResetPassword from '../pages/auth/ResetPassword'
import PanelLayout from '../layouts/PanelLayout'
import ProtectedRoute from '../components/ProtectedRoute'
import Users from '../pages/users/Users'
import UserDetail from '../pages/users/UserDetail'
import UserInfo from '../pages/users/UserInfo'
import Permissions from '../pages/permissions/Permissions'
import AuditLogs from '../pages/auditLogs/AuditLogs'
import Dashboard from '../pages/dashboard/Dashboard'
import CreateUser from '../pages/users/CreateUser'
import ChangePassword from '../pages/auth/ChangePassword'
import Region from '../pages/region/Region'
import Brand from '../pages/brand/Brand'
import City from '../pages/city/City'
import Vehicle from '../pages/vehicle/Vehicle'
import Company from '../pages/company/Company'
import Agency from '../pages/agency/Agency'
import Demographic from '../pages/demographic/Demographic'
import ExecSummary from '../pages/execSummary/ExecSummary'
import CustomSegment from '../pages/customSegment/CustomSegment'
import SavedSegments from '../pages/customSegment/SavedSegments'

const router = createBrowserRouter([
  { path: '/login', element: <Login /> },
  { path: '/mfa', element: <Mfa /> },
  { path: '/mfa-setup', element: <MfaSetup /> },
  { path: '/forgot-password', element: <ForgotPassword /> },
  { path: '/reset-password', element: <ResetPassword /> },
  { path: '/change-password', element: <ChangePassword /> },
  {
    path: '/',
    element: <ProtectedRoute />,
    children: [
      {
        element: <PanelLayout />,
        children: [
          { path: 'dashboard', element: <Dashboard /> },
          { path: 'exec-summary', element: <ExecSummary /> },
          { path: 'custom-segment', element: <CustomSegment /> },
          { path: 'custom-segment/saved', element: <SavedSegments /> },
          { path: 'region', element: <Region /> },
          { path: 'brand', element: <Brand /> },
          { path: 'city', element: <City /> },
          { path: 'vehicle', element: <Vehicle /> },
          { path: 'company', element: <Company /> },
          { path: 'agency', element: <Agency /> },
          { path: 'demographic', element: <Demographic /> },
          { path: 'users', element: <Users /> },
          { path: 'users/:id', element: <UserDetail /> },
          { path: 'permissions', element: <Permissions /> },
          { path: 'audit-logs', element: <AuditLogs /> },
          { path: 'account', element: <UserInfo /> },
          { path: 'users/new', element: <CreateUser /> },
        ],
      },
    ],
  },
])

export default router