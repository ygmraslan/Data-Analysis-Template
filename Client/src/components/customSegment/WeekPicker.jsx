import { useEffect, useState } from 'react'
import { getAvailableWeeks, getCurrentWeek } from '../../api/execSummaryApi'
import { SingleDropdown } from './Dropdown'

const MONTHS_SHORT = ['Oca', 'Şub', 'Mar', 'Nis', 'May', 'Haz', 'Tem', 'Ağu', 'Eyl', 'Eki', 'Kas', 'Ara']

const MONTHS_TR = [
  { value: 1, label: 'Ocak' }, { value: 2, label: 'Şubat' }, { value: 3, label: 'Mart' },
  { value: 4, label: 'Nisan' }, { value: 5, label: 'Mayıs' }, { value: 6, label: 'Haziran' },
  { value: 7, label: 'Temmuz' }, { value: 8, label: 'Ağustos' }, { value: 9, label: 'Eylül' },
  { value: 10, label: 'Ekim' }, { value: 11, label: 'Kasım' }, { value: 12, label: 'Aralık' }
]

function formatWeekLabel(start, end) {
  if (!start || !end) return ''
  const s = new Date(start)
  const e = new Date(end)
  const sameMonth = s.getMonth() === e.getMonth() && s.getFullYear() === e.getFullYear()
  if (sameMonth) {
    return `${s.getDate()} - ${e.getDate()} ${MONTHS_SHORT[e.getMonth()]} ${e.getFullYear()}`
  }
  return `${s.getDate()} ${MONTHS_SHORT[s.getMonth()]} - ${e.getDate()} ${MONTHS_SHORT[e.getMonth()]} ${e.getFullYear()}`
}

export default function WeekPicker({ value, onChange, initialDate, layout = 'inline' }) {
  const now = new Date()
  const currentYear = now.getFullYear()
  const currentMonth = now.getMonth() + 1

  const [year, setYear] = useState(value?.year ?? currentYear)
  const [month, setMonth] = useState(value?.month ?? currentMonth)
  const [weeks, setWeeks] = useState([])
  const [weeksLoading, setWeeksLoading] = useState(true)

  const years = Array.from({ length: 3 }, (_, i) => currentYear - i)

  useEffect(() => {
    let cancelled = false

    const loadInitial = async () => {
      try {
        if (initialDate) {
          const d = new Date(initialDate)
          const y = d.getFullYear()
          const m = d.getMonth() + 1
          const res = await getAvailableWeeks(y, m)
          if (cancelled) return
          const list = res.data || []
          setYear(y)
          setMonth(m)
          setWeeks(list)
          const match = list.find(w => w.endDate?.slice(0, 10) === initialDate.slice(0, 10))
          const chosen = match || pickLastPastWeek(list)
          if (chosen && onChange) onChange(chosen)
        } else {
          const res = await getCurrentWeek()
          if (cancelled) return
          const week = res.data
          setYear(week.year)
          setMonth(week.month)
          const weeksRes = await getAvailableWeeks(week.year, week.month)
          if (cancelled) return
          setWeeks(weeksRes.data || [])
          if (onChange) onChange(week)
        }
      } catch (err) {
        console.error('WeekPicker initial load error:', err)
      } finally {
        if (!cancelled) setWeeksLoading(false)
      }
    }

    loadInitial()
    return () => { cancelled = true }
  }, [])

  const handleYearChange = async (newYear) => {
    setYear(newYear)
    await reloadWeeks(newYear, month)
  }

  const handleMonthChange = async (newMonth) => {
    setMonth(newMonth)
    await reloadWeeks(year, newMonth)
  }

  const reloadWeeks = async (y, m) => {
    setWeeksLoading(true)
    try {
      const res = await getAvailableWeeks(y, m)
      const list = res.data || []
      setWeeks(list)
      const chosen = pickLastPastWeek(list)
      if (onChange) onChange(chosen)
    } catch (err) {
      console.error('Week reload error:', err)
    } finally {
      setWeeksLoading(false)
    }
  }

  const handleWeekChange = (weekIndex) => {
    const w = weeks.find(x => x.weekIndex === weekIndex)
    if (w && onChange) onChange(w)
  }

  const yearOptions = years.map(y => ({ value: y, label: String(y) }))
  const monthOptions = MONTHS_TR.filter(m => !(year === currentYear && m.value > currentMonth))
  const weekOptions = weeksLoading
    ? [{ value: null, label: 'Yükleniyor...' }]
    : weeks
        .filter(w => new Date(w.endDate) <= now)
        .map(w => ({ value: w.weekIndex, label: formatWeekLabel(w.startDate, w.endDate) }))

  const gridClass = layout === 'stacked'
    ? 'grid grid-cols-1 gap-3'
    : 'grid grid-cols-1 sm:grid-cols-3 gap-3'

  return (
    <div className={gridClass}>
      <SingleDropdown
        label="Yıl"
        value={year}
        options={yearOptions}
        onChange={handleYearChange}
      />
      <SingleDropdown
        label="Ay"
        value={month}
        options={monthOptions}
        onChange={handleMonthChange}
      />
      <SingleDropdown
        label="Hafta"
        value={value?.weekIndex}
        options={weekOptions}
        onChange={handleWeekChange}
        loading={weeksLoading}
        placeholder="Hafta seçin"
      />
    </div>
  )
}

function pickLastPastWeek(list) {
  if (!list || list.length === 0) return null
  const now = new Date()
  const past = list.filter(w => new Date(w.endDate) <= now)
  if (past.length > 0) return past[past.length - 1]
  return list[0]
}