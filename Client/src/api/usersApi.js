import axiosInstance from './axiosInstance'

const usersApi = {
  getUsers: (params) => axiosInstance.get('/users', { params }),
  getUserById: (id) => axiosInstance.get(`/users/${id}`),
  createUser: (data) => axiosInstance.post('/users', data),
  updateUser: (id, data) => axiosInstance.put(`/users/${id}`, data),
  toggleStatus: (id) => axiosInstance.patch(`/users/${id}/toggle-status`),
  unlockUser: (id) => axiosInstance.patch(`/users/${id}/unlock`),
  resetMfa: (id) => axiosInstance.patch(`/users/${id}/reset-mfa`),
  resetPassword: (id) => axiosInstance.patch(`/users/${id}/reset-password`),
}

export default usersApi