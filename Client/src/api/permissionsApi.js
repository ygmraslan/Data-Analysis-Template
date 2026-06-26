import axiosInstance from './axiosInstance'

const permissionsApi = {
  getPermissions: (userId) => axiosInstance.get(`/permissions/${userId}`),
  assignPermissions: (data) => axiosInstance.post('/permissions/assign', data),
}

export default permissionsApi