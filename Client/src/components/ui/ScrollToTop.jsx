import { useState, useEffect } from 'react'

export default function ScrollToTop() {
  const [visible, setVisible] = useState(false)

  useEffect(() => {
    const onScroll = () => setVisible(window.scrollY > 300)
    window.addEventListener('scroll', onScroll)
    return () => window.removeEventListener('scroll', onScroll)
  }, [])

  if (!visible) return null

  return (
    <button
      onClick={() => window.scrollTo({ top: 0, behavior: 'smooth' })}
      className="fixed bottom-6 right-6 z-50 w-9 h-9 flex items-center justify-center
        bg-white dark:bg-[#0d1f3c]
        border border-slate-200 dark:border-white/10
        rounded-lg shadow-md
        text-slate-500 dark:text-slate-400
        hover:text-slate-800 dark:hover:text-white
        hover:border-slate-300 dark:hover:border-white/20
        transition-all"
      aria-label="Yukarı çık"
    >
      <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
        <polyline points="18 15 12 9 6 15" />
      </svg>
    </button>
  )
}