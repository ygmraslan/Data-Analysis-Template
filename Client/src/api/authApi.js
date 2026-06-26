import axiosInstance from './axiosInstance'

const authApi = {
  login: (data) =>
    axiosInstance.post('/auth/login', data),
  
  setupMfa: (data) =>
  axiosInstance.post('/auth/setup-mfa', data),

  confirmMfa: (data) =>
    axiosInstance.post('/auth/verify-mfa', data),

  logout: (data) =>
    axiosInstance.post('/auth/logout', data),

  changePassword: (data) =>
    axiosInstance.post('/auth/change-password', data),

  forgotPassword: (data) =>
    axiosInstance.post('/auth/forgot-password', data),

  resetPassword: (data) =>
    axiosInstance.post('/auth/reset-password', data),

  refreshToken: (data) =>
    axiosInstance.post('/auth/token', data),
}

export default authApi
