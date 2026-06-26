import axiosInstance from './axiosInstance'
import { buildFilterParams } from '../utils/detailFilter'

export const getKpi = (productGroup = 'KASKO', filter = null) =>
  axiosInstance.get('/dashboard/kpi', { params: buildFilterParams(productGroup, filter) })

export const getSegmentDrift = (productGroup = 'KASKO', filter = null) =>
  axiosInstance.get('/dashboard/segment-drift', { params: buildFilterParams(productGroup, filter) })

export const getDistribution = (productGroup = 'KASKO', distributionType = 'Brand', filter = null) => {
  const params = buildFilterParams(productGroup, filter)
  params.append('distributionType', distributionType)
  return axiosInstance.get('/dashboard/distribution', { params })
}

export const getHeatmap = (productGroup = 'KASKO', filter = null) =>
  axiosInstance.get('/dashboard/heatmap', { params: buildFilterParams(productGroup, filter) })

export const getWeeklyTotals = (productGroup = 'KASKO', filter = null) =>
  axiosInstance.get('/dashboard/weekly-totals', { params: buildFilterParams(productGroup, filter) })