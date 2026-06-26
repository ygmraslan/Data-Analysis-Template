import axiosInstance from './axiosInstance'

export const getAvailableWeeks = (year, month) =>
  axiosInstance.get('/execsummary/available-weeks', { params: { year, month } })

export const getCurrentWeek = () =>
  axiosInstance.get('/execsummary/current-week')

export const getExecDrift = (productGroup = 'KASKO', startDate, endDate) =>
  axiosInstance.get('/execsummary/drift', { params: { productGroup, startDate, endDate } })

export const getExecBrandAge = (productGroup = 'KASKO', startDate, endDate) =>
  axiosInstance.get('/execsummary/brand-age', { params: { productGroup, startDate, endDate } })

export const getExecAgeStep = (productGroup = 'KASKO', startDate, endDate) =>
  axiosInstance.get('/execsummary/age-step', { params: { productGroup, startDate, endDate } })

export const getExecYoungDriver = (productGroup = 'KASKO', startDate, endDate) =>
  axiosInstance.get('/execsummary/young-driver', { params: { productGroup, startDate, endDate } })

export const getExecRisk = (productGroup = 'KASKO', startDate, endDate) =>
  axiosInstance.get('/execsummary/risk', { params: { productGroup, startDate, endDate } })

export const getExecDistribution = (productGroup = 'KASKO', startDate, endDate) =>
  axiosInstance.get('/execsummary/distribution', { params: { productGroup, startDate, endDate } })

const AI_TIMEOUT_MS = 200000

const isRetryableNetworkError = (err) => {
  if (!err) return false
  if (err.code === 'ECONNABORTED') return false
  if (err.message?.toLowerCase().includes('timeout')) return false
  if (err.response) return false
  return err.code === 'ERR_NETWORK' ||
         err.code === 'ECONNRESET' ||
         err.message === 'Network Error'
}

const aiPostWithRetry = async (url, params) => {
  try {
    return await axiosInstance.post(url, null, { params, timeout: AI_TIMEOUT_MS })
  } catch (err) {
    if (isRetryableNetworkError(err)) {
      await new Promise(r => setTimeout(r, 2000))
      return await axiosInstance.post(url, null, { params, timeout: AI_TIMEOUT_MS })
    }
    throw err
  }
}

export const getExecAiSummary = (productGroup = 'KASKO', startDate, endDate, forceRefresh = false) =>
  aiPostWithRetry('/execsummary/ai-summary', { productGroup, startDate, endDate, forceRefresh })

export const getExecAiDeepSeek = (productGroup = 'KASKO', startDate, endDate, forceRefresh = false) =>
  aiPostWithRetry('/execsummary/ai-deepseek', { productGroup, startDate, endDate, forceRefresh })

export const getExecAiGemini = (productGroup = 'KASKO', startDate, endDate, forceRefresh = false) =>
  aiPostWithRetry('/execsummary/ai-gemini', { productGroup, startDate, endDate, forceRefresh })

export const getExecAiGpt = (productGroup = 'KASKO', startDate, endDate, forceRefresh = false) =>
  aiPostWithRetry('/execsummary/ai-gpt', { productGroup, startDate, endDate, forceRefresh })

export const exportExecSummaryPdf = (productGroup = 'KASKO', startDate, endDate) =>
  axiosInstance.get('/execsummary/export/pdf', { 
    params: { productGroup, startDate, endDate },
    responseType: 'blob'
  })