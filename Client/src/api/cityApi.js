import axiosInstance from './axiosInstance'
import { buildFilterParams } from '../utils/detailFilter'

export const getCityKpi = (productGroup = 'KASKO', filter = null) =>
  axiosInstance.get('/city/kpi', { params: buildFilterParams(productGroup, filter) })

export const getCityList = (productGroup = 'KASKO', filter = null) =>
  axiosInstance.get('/city/list', { params: buildFilterParams(productGroup, filter) })

export const getCityTrend = (productGroup = 'KASKO', city = '', filter = null) => {
  const params = buildFilterParams(productGroup, filter)
  params.append('city', city)
  return axiosInstance.get('/city/trend', { params })
}

export const getCityProfile = (productGroup = 'KASKO', city = '', filter = null) => {
  const params = buildFilterParams(productGroup, filter)
  params.append('city', city)
  return axiosInstance.get('/city/profile', { params })
}

export const getCityHeatmap = (productGroup = 'KASKO', filter = null) =>
  axiosInstance.get('/city/heatmap', { params: buildFilterParams(productGroup, filter) })