export function formatDate(date) {
  if (!date) return '—'
  return new Date(date).toLocaleDateString('tr-TR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

export function formatDateOnly(date) {
  if (!date) return '—'
  return new Date(date).toLocaleDateString('tr-TR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric'
  })
}

// Week Operations
 
function getMonday(date) {
  const d = new Date(date)
  const day = d.getDay()
  const diff = day === 0 ? 6 : day - 1
  d.setDate(d.getDate() - diff)
  d.setHours(0, 0, 0, 0)
  return d
}
 
function fmt(d) {
  return `${String(d.getDate()).padStart(2, '0')}.${String(d.getMonth() + 1).padStart(2, '0')}`
}
 
function fmtFull(d) {
  return `${String(d.getDate()).padStart(2, '0')}.${String(d.getMonth() + 1).padStart(2, '0')}.${d.getFullYear()}`
}
 
function addDays(date, days) {
  const d = new Date(date)
  d.setDate(d.getDate() + days)
  return d
}
 
// KPI
export function getLastWeekRange() {
  const thisMonday = getMonday(new Date())
  const lastMonday = addDays(thisMonday, -7)
  const lastSunday = addDays(lastMonday, 6)
  return `${fmt(lastMonday)} — ${fmtFull(lastSunday)}`
}
 
//KPI (LAST WEEK)
export function getPrevWeekRange() {
  const thisMonday  = getMonday(new Date())
  const twoWeeksAgo = addDays(thisMonday, -14)
  const prevSunday  = addDays(twoWeeksAgo, 6)
  return `${fmt(twoWeeksAgo)} — ${fmtFull(prevSunday)}`
}
 
// Heatmap - Trend
export function getEightWeekRange() {
  const thisMonday     = getMonday(new Date())
  const lastMonday     = addDays(thisMonday, -7)
  const lastSunday     = addDays(lastMonday, 6)
  const eightWeeksAgo  = addDays(lastMonday, -49)
  return `${fmt(eightWeeksAgo)} — ${fmtFull(lastSunday)}`
}
 
// Segment Drift
export function getNineWeekRange() {
  const thisMonday    = getMonday(new Date())
  const lastMonday    = addDays(thisMonday, -7)
  const lastSunday    = addDays(lastMonday, 6)
  const nineWeeksAgo  = addDays(lastMonday, -56)
  return `${fmt(nineWeeksAgo)} — ${fmtFull(lastSunday)}`
}