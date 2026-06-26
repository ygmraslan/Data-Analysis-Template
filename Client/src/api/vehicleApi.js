import axiosInstance from './axiosInstance'
import { buildFilterParams } from '../utils/detailFilter'

export const getVehicleKpi = (productGroup = 'KASKO', filter = null) =>
  axiosInstance.get('/vehicle/kpi', { params: buildFilterParams(productGroup, filter) })

export const getVehicleAge = (productGroup = 'KASKO', grouped = true, filter = null) => {
  const params = buildFilterParams(productGroup, filter)
  params.append('grouped', grouped)
  return axiosInstance.get('/vehicle/age', { params })
}

export const getVehiclePrice = (productGroup = 'KASKO', filter = null) =>
  axiosInstance.get('/vehicle/price', { params: buildFilterParams(productGroup, filter) })

export const getVehicleBody = (productGroup = 'KASKO', filter = null) =>
  axiosInstance.get('/vehicle/body', { params: buildFilterParams(productGroup, filter) })

export const getVehicleSegment = (productGroup = 'KASKO', filter = null) =>
  axiosInstance.get('/vehicle/segment', { params: buildFilterParams(productGroup, filter) })

export const getVehicleAgeHeatmap = (productGroup = 'KASKO', filter = null) =>
  axiosInstance.get('/vehicle/heatmap/age', { params: buildFilterParams(productGroup, filter) })

export const getVehiclePriceHeatmap = (productGroup = 'KASKO', filter = null) =>
  axiosInstance.get('/vehicle/heatmap/price', { params: buildFilterParams(productGroup, filter) })

export const getVehicleTrend = (productGroup = 'KASKO', trendType = 'Age', group = '', grouped = true, filter = null) => {
  const params = buildFilterParams(productGroup, filter)
  params.append('trendType', trendType)
  params.append('group', group)
  params.append('grouped', grouped)
  return axiosInstance.get('/vehicle/trend', { params })
}