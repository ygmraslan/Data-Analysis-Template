import axiosInstance from './axiosInstance'
import { buildFilterParams } from '../utils/detailFilter'

export const getBrandKpi = (productGroup = 'KASKO', filter = null) =>
  axiosInstance.get('/brand/kpi', { params: buildFilterParams(productGroup, filter) })

export const getBrandList = (productGroup = 'KASKO', filter = null) =>
  axiosInstance.get('/brand/list', { params: buildFilterParams(productGroup, filter) })

export const getBrandTrend = (productGroup = 'KASKO', brand = '', filter = null) => {
  const params = buildFilterParams(productGroup, filter)
  params.append('brand', brand)
  return axiosInstance.get('/brand/trend', { params })
}

export const getBrandModels = (productGroup = 'KASKO', brand = '', filter = null) => {
  const params = buildFilterParams(productGroup, filter)
  params.append('brand', brand)
  return axiosInstance.get('/brand/models', { params })
}

export const getBrandHeatmap = (productGroup = 'KASKO', filter = null) =>
  axiosInstance.get('/brand/heatmap', { params: buildFilterParams(productGroup, filter) })