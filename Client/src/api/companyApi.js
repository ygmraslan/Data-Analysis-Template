import axiosInstance from './axiosInstance'
import { buildFilterParams } from '../utils/detailFilter'

export const getCompanyKpi = (productGroup = 'KASKO', filter = null) =>
  axiosInstance.get('/company/kpi', { params: buildFilterParams(productGroup, filter) })

export const getCompanyList = (productGroup = 'KASKO', filter = null) =>
  axiosInstance.get('/company/list', { params: buildFilterParams(productGroup, filter) })

export const getCompanyTrend = (productGroup = 'KASKO', company = '', filter = null) => {
  const params = buildFilterParams(productGroup, filter)
  params.append('company', company)
  return axiosInstance.get('/company/trend', { params })
}

export const getCompanyRenewal = (productGroup = 'KASKO', filter = null) =>
  axiosInstance.get('/company/renewal', { params: buildFilterParams(productGroup, filter) })

export const getRenewalSteps = (productGroup = 'KASKO', renewalType = 'Yeni İş', filter = null) => {
  const params = buildFilterParams(productGroup, filter)
  params.append('renewalType', renewalType)
  return axiosInstance.get('/company/renewal/steps', { params })
}

export const getCompanyProfile = (productGroup = 'KASKO', company = '', filter = null) => {
  const params = buildFilterParams(productGroup, filter)
  params.append('company', company)
  return axiosInstance.get('/company/profile', { params })
}

export const getCompanyHeatmap = (productGroup = 'KASKO', filter = null) =>
  axiosInstance.get('/company/heatmap', { params: buildFilterParams(productGroup, filter) })