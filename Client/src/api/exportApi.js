import axiosInstance from './axiosInstance'
import { buildFilterParams } from '../utils/detailFilter'

const blob = (url, productGroup, filter) =>
  axiosInstance.get(url, {
    params: buildFilterParams(productGroup, filter),
    responseType: 'blob',
  })

export const exportHeatmap = (productGroup = 'KASKO', filter = null) =>
  blob('/dashboard/export/heatmap', productGroup, filter)

export const exportRegionHeatmap = (productGroup = 'KASKO', filter = null) =>
  blob('/region/export/heatmap', productGroup, filter)

export const exportRegionReport = (productGroup = 'KASKO', filter = null) =>
  blob('/region/export/report', productGroup, filter)

export const exportBrandHeatmap = (productGroup = 'KASKO', filter = null) =>
  blob('/brand/export/heatmap', productGroup, filter)

export const exportBrandReport = (productGroup = 'KASKO', filter = null) =>
  blob('/brand/export/report', productGroup, filter)

export const exportCityHeatmap = (productGroup = 'KASKO', filter = null) =>
  blob('/city/export/heatmap', productGroup, filter)

export const exportCityReport = (productGroup = 'KASKO', filter = null) =>
  blob('/city/export/report', productGroup, filter)

export const exportVehicleAgeHeatmap = (productGroup = 'KASKO', filter = null) =>
  blob('/vehicle/export/heatmap/age', productGroup, filter)

export const exportVehiclePriceHeatmap = (productGroup = 'KASKO', filter = null) =>
  blob('/vehicle/export/heatmap/price', productGroup, filter)

export const exportVehicleReport = (productGroup = 'KASKO', filter = null) =>
  blob('/vehicle/export/report', productGroup, filter)

export const exportCompanyHeatmap = (productGroup = 'KASKO', filter = null) =>
  blob('/company/export/heatmap', productGroup, filter)

export const exportCompanyReport = (productGroup = 'KASKO', filter = null) =>
  blob('/company/export/report', productGroup, filter)

export const exportAgencyHeatmap = (productGroup = 'KASKO', filter = null) =>
  blob('/agency/export/heatmap', productGroup, filter)

export const exportAgencyReport = (productGroup = 'KASKO', filter = null) =>
  blob('/agency/export/report', productGroup, filter)