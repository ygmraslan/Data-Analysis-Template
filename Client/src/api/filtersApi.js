import axiosInstance from './axiosInstance'

export const getFilterOptions = () => axiosInstance.get('/filters/options')