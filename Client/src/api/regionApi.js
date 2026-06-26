import axiosInstance from './axiosInstance'
import { buildFilterParams } from '../utils/detailFilter'

export const getRegionKpi = (productGroup = 'KASKO', filter = null) =>
  axiosInstance.get('/region/kpi', { params: buildFilterParams(productGroup, filter) })

export const getRegionTrend = (productGroup = 'KASKO', filter = null) =>
  axiosInstance.get('/region/trend', { params: buildFilterParams(productGroup, filter) })

export const getRegionHeatmap = (productGroup = 'KASKO', filter = null) =>
  axiosInstance.get('/region/heatmap', { params: buildFilterParams(productGroup, filter) })