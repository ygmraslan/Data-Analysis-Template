const TUZEL_MATCHERS = ['TUZEL', 'TÜZEL']
export const TUZEL_AGE_BAND = '0-17'

export const isTuzelSelected = (insuredTypes) => {
  if (!Array.isArray(insuredTypes) || insuredTypes.length === 0) return false
  return insuredTypes.some((t) => {
    if (!t) return false
    const normalized = String(t).toUpperCase()
    return TUZEL_MATCHERS.some((variant) => normalized.includes(variant))
  })
}
export const applyInsuredTypeRule = (prevFilters, key, value) => {
  const next = { ...prevFilters, [key]: value }
  if (key !== 'insuredTypes') return next

  const wasTuzel = isTuzelSelected(prevFilters?.insuredTypes)
  const isTuzelNow = isTuzelSelected(value)

  if (isTuzelNow && !wasTuzel) {
    next.insuredAges = [TUZEL_AGE_BAND]
  } else if (!isTuzelNow && wasTuzel) {
    next.insuredAges = []
  }
  return next
}