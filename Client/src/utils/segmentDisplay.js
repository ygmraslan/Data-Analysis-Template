const MONTHS_SHORT = [
  'Oca', 'Şub', 'Mar', 'Nis', 'May', 'Haz',
  'Tem', 'Ağu', 'Eyl', 'Eki', 'Kas', 'Ara'
]

export function formatWeekRange(start, end) {
  if (!start || !end) return '-'
  const s = new Date(start)
  const e = new Date(end)
  return `${s.getDate()} ${MONTHS_SHORT[s.getMonth()]} – ${e.getDate()} ${MONTHS_SHORT[e.getMonth()]} ${e.getFullYear()}`
}

export function getFilterParts(filters) {
  if (!filters) return []
  const parts = []
  if (filters.brands?.length) parts.push(...filters.brands)
  if (filters.insuredAges?.length) parts.push(...filters.insuredAges)
  if (filters.insuredTypes?.length) parts.push(...filters.insuredTypes)
  if (filters.genders?.length) {
    parts.push(...filters.genders.map(g => g === 'E' ? 'Erkek' : 'Kadın'))
  }
  if (filters.vehicleAges?.length) parts.push(...filters.vehicleAges)
  if (filters.vehicleValues?.length) parts.push(...filters.vehicleValues)
  return parts
}