import useAuthStore from '../store/authStore'

export function usePermission() {
  const { user } = useAuthStore()
  const permissions = user?.permissions || []

  const hasPermission = (permission) => permissions.includes(permission)
  const hasAnyPermission = (perms) => perms.some(p => permissions.includes(p))

  return { hasPermission, hasAnyPermission }
}