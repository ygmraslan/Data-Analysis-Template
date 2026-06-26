export const EMPTY_FILTER = {
  insuredTypes: [],
  businessSources: [],
  vehicleTypes: [],
  productCodes: [],
}

export function buildFilterParams(productGroup, filter) {
  const p = new URLSearchParams()
  if (productGroup) p.append('productGroup', productGroup)
  if (filter) {
    ;(filter.insuredTypes    || []).forEach(v => p.append('insuredType', v))
    ;(filter.businessSources || []).forEach(v => p.append('businessSource', v))
    ;(filter.vehicleTypes    || []).forEach(v => p.append('vehicleType', v))
    ;(filter.productCodes    || []).forEach(v => p.append('product', v))
  }
  return p
}

export function isFilterActive(filter) {
  if (!filter) return false
  return (
    (filter.insuredTypes?.length    || 0) +
    (filter.businessSources?.length || 0) +
    (filter.vehicleTypes?.length    || 0) +
    (filter.productCodes?.length    || 0)
  ) > 0
}

export const stripBusinessSource = (filter) => ({ ...filter, businessSources: [] })