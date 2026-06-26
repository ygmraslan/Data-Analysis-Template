import axiosInstance from './axiosInstance'
import { buildFilterParams } from '../utils/detailFilter'

export const getAgencyKpi = (productGroup = 'KASKO', filter = null) =>
  axiosInstance.get('/agency/kpi', { params: buildFilterParams(productGroup, filter) })

export const getAgencyList = (productGroup = 'KASKO', page = 1, pageSize = 20, region = null, filter = null) => {
  const params = buildFilterParams(productGroup, filter)
  params.append('page', page)
  params.append('pageSize', pageSize)
  if (region) params.append('region', region)
  return axiosInstance.get('/agency/list', { params })
}

export const getAgencyTrend = (productGroup = 'KASKO', agencyCode = '', filter = null) => {
  const params = buildFilterParams(productGroup, filter)
  params.append('agencyCode', agencyCode)
  return axiosInstance.get('/agency/trend', { params })
}

export const getAgencyProfile = (productGroup = 'KASKO', agencyCode = '', filter = null) => {
  const params = buildFilterParams(productGroup, filter)
  params.append('agencyCode', agencyCode)
  return axiosInstance.get('/agency/profile', { params })
}

export const getAgencyRegion = (productGroup = 'KASKO', filter = null) =>
  axiosInstance.get('/agency/region', { params: buildFilterParams(productGroup, filter) })

export const getAgencyHeatmap = (productGroup = 'KASKO', page = 1, pageSize = 100, filter = null) => {
  const params = buildFilterParams(productGroup, filter)
  params.append('page', page)
  params.append('pageSize', pageSize)
  return axiosInstance.get('/agency/heatmap', { params })
}