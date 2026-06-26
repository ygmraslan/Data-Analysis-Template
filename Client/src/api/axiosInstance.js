import axios from 'axios'
import { getErrorMessage } from '../utils/errorMessages'

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5056/api'

const axiosInstance = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
  withCredentials: true,
})

let isRefreshing = false
let pendingRequests = []

const subscribeRefresh = (cb) => {
  pendingRequests.push(cb)
}

const notifyRefreshSuccess = () => {
  pendingRequests.forEach(cb => cb(null))
  pendingRequests = []
}

const notifyRefreshFailed = (err) => {
  pendingRequests.forEach(cb => cb(err))
  pendingRequests = []
}

axiosInstance.interceptors.response.use(
  (response) => response,
  async (error) => {
    const original = error.config

    if (
      error.response?.status === 401 &&
      original &&
      !original._retry &&
      !original.url.includes('/auth/login') &&
      !original.url.includes('/auth/verify-mfa') &&
      !original.url.includes('/auth/setup-mfa') &&
      !original.url.includes('/auth/refresh-token')
    ) {
      original._retry = true

      if (isRefreshing) {
        return new Promise((resolve, reject) => {
          subscribeRefresh((err) => {
            if (err) {
              reject(err)
              return
            }
            resolve(axiosInstance(original))
          })
        })
      }

      isRefreshing = true

      try {
        await axios.post(
          `${API_BASE_URL}/auth/refresh-token`,
          {},
          { withCredentials: true }
        )
        notifyRefreshSuccess()
        return axiosInstance(original)
      } catch (refreshErr) {
        notifyRefreshFailed(refreshErr)
        window.location.href = '/login'
        return Promise.reject(refreshErr)
      } finally {
        isRefreshing = false
      }
    }

    const errorCode = error.response?.data?.code
    if (errorCode) {
      error.friendlyMessage = getErrorMessage(errorCode)
    } else {
      error.friendlyMessage = error.response?.data?.message || getErrorMessage('default')
    }

    return Promise.reject(error)
  }
)

export default axiosInstance