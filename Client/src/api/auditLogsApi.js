import axiosInstance from './axiosInstance'

const auditLogsApi = {
  getLogs:     (params) => axiosInstance.get('/audit-logs', { params }),
  getAuthLogs: (params) => axiosInstance.get('/auth-logs', { params }),
}

export default auditLogsApi