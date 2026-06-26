import { useEffect, useState } from 'react'
import { Navigate, Outlet } from 'react-router-dom'
import useAuthStore from '../store/authStore'
import axiosInstance from '../api/axiosInstance'

export default function ProtectedRoute() {
  const { isAuthenticated, setAuthenticated, setUser } = useAuthStore()
  const [isChecking, setIsChecking] = useState(true)

  useEffect(() => {
    const checkAuth = async () => {
      try {
        const response = await axiosInstance.get('/auth/me')
        setUser(response.data.value)
        setAuthenticated(true)
      } catch {
        setAuthenticated(false)
      } finally {
        setIsChecking(false)
      }
    }

    checkAuth()
  }, [])

  if (isChecking) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-slate-950">
        <svg className="animate-spin text-emerald-400" width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
          <path d="M21 12a9 9 0 1 1-6.219-8.56" />
        </svg>
      </div>
    )
  }

  return isAuthenticated ? <Outlet /> : <Navigate to="/login" replace />
}