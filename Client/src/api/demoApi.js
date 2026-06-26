import axiosInstance from './axiosInstance'
import { buildFilterParams } from '../utils/detailFilter'

export const getDemoKpi = (productGroup = 'KASKO', filter = null) =>
  axiosInstance.get('/demo/kpi', { params: buildFilterParams(productGroup, filter) })

export const getDemoInsuredType = (productGroup = 'KASKO', filter = null) =>
  axiosInstance.get('/demo/insured-type', { params: buildFilterParams(productGroup, filter) })

export const getDemoGender = (productGroup = 'KASKO', filter = null) =>
  axiosInstance.get('/demo/gender', { params: buildFilterParams(productGroup, filter) })

export const getDemoAgeGroup = (productGroup = 'KASKO', filter = null) =>
  axiosInstance.get('/demo/age-group', { params: buildFilterParams(productGroup, filter) })

export const getDemoInsuredCity = (productGroup = 'KASKO', filter = null) =>
  axiosInstance.get('/demo/insured-city', { params: buildFilterParams(productGroup, filter) })

export const exportDemoReport = (productGroup = 'KASKO') =>
  axiosInstance.get('/demo/export/report', {
    params: { productGroup },
    responseType: 'blob'
  })