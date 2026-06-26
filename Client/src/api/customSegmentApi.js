import axiosInstance from './axiosInstance'

export const getOptions = (productGroup = 'KASKO') =>
  axiosInstance.get('/custom-segment/options', { params: { productGroup } })

export const calculateDrift = (data) =>
  axiosInstance.post('/custom-segment/calculate', data)

export const getSegments = (productGroup, search) =>
  axiosInstance.get('/custom-segment', { params: { productGroup, search } })

export const getSegmentById = (id) =>
  axiosInstance.get(`/custom-segment/${id}`)

export const saveSegment = (data) =>
  axiosInstance.post('/custom-segment', data)

export const deleteSegment = (id) =>
  axiosInstance.delete(`/custom-segment/${id}`)

export const runSegment = (id, weekStart, weekEnd) =>
  axiosInstance.post(`/custom-segment/${id}/run`, { weekStart, weekEnd })

export const getSegmentHistory = (id) =>
  axiosInstance.get(`/custom-segment/${id}/history`)

export const getComparisons = (productGroup) =>
  axiosInstance.get('/custom-segment/comparisons', { params: { productGroup } })

export const getComparisonById = (id) =>
  axiosInstance.get(`/custom-segment/comparisons/${id}`)

export const saveComparison = (data) =>
  axiosInstance.post('/custom-segment/comparisons', data)

export const deleteComparison = (id) =>
  axiosInstance.delete(`/custom-segment/comparisons/${id}`)

export const runComparison = (id, weekStart, weekEnd) =>
  axiosInstance.post(`/custom-segment/comparisons/${id}/run`, { weekStart, weekEnd })

export const getCalculateAiComment = (model, data) =>
  axiosInstance.post(`/custom-segment/calculate/ai/${model}`, data)

export const getCompareAiComment = (model, data) =>
  axiosInstance.post(`/custom-segment/calculate-compare/ai/${model}`, data)