import { useState } from 'react'

export default function AuthCard({ children, error }) {
  const [clickEffects, setClickEffects] = useState([])

  const handleBackgroundClick = (e) => {
    const rect = e.currentTarget.getBoundingClientRect()
    const id = Date.now()
    setClickEffects(prev => [...prev, { id, x: e.clientX - rect.left, y: e.clientY - rect.top }])
    setTimeout(() => setClickEffects(prev => prev.filter(ef => ef.id !== id)), 800)
  }

  return (
    <div
      className="min-h-screen flex items-center justify-center p-6 bg-gradient-to-b from-slate-800 to-slate-950"
      style={{ cursor: 'crosshair' }}
      onClick={handleBackgroundClick}
    >
      {clickEffects.map(ef => (
        <div
          key={ef.id}
          className="fixed pointer-events-none z-0 w-1 rounded"
          style={{
            left: ef.x,
            top: ef.y,
            background: 'linear-gradient(to top, #0ea5e9, #34d399)',
            boxShadow: '0 0 10px rgba(52,211,153,0.8)',
            animation: 'riseUp 0.8s cubic-bezier(0.1,0.8,0.3,1) forwards',
          }}
        />
      ))}

      <div
        className="relative z-10 flex w-full rounded-3xl overflow-hidden shadow-2xl"
        style={{ maxWidth: 1100, minHeight: 640, cursor: 'default' }}
        onClick={e => e.stopPropagation()}
      >
        {/* Sol Panel */}
        <div className="hidden md:flex flex-1 relative items-center justify-center overflow-hidden bg-slate-950">
          <div className="absolute inset-0 bg-[radial-gradient(circle_at_50%_50%,rgba(52,211,153,0.05),transparent_70%)]" />

          <svg className="absolute pointer-events-none" style={{ top: '32%', right: '12%', width: 140, height: 70 }} viewBox="0 0 100 50">
            <path d="M5,45 L35,25 L55,35 L95,5" fill="none" stroke="#34d399" strokeWidth="3.5" strokeLinecap="round" strokeLinejoin="round"
              style={{ strokeDasharray: 200, strokeDashoffset: 200, animation: 'drawLine 3s ease-in-out infinite alternate' }} />
            <circle cx="95" cy="5" r="3" fill="#34d399" />
          </svg>

          <svg className="absolute pointer-events-none" style={{ bottom: '35%', left: '10%', width: 110, height: 55 }} viewBox="0 0 100 50">
            <path d="M5,5 L35,25 L55,15 L95,45" fill="none" stroke="#f43f5e" strokeWidth="3.5" strokeLinecap="round" strokeLinejoin="round"
              style={{ strokeDasharray: 200, strokeDashoffset: 200, animation: 'drawLine 3.5s ease-in-out infinite alternate-reverse' }} />
            <circle cx="95" cy="45" r="3" fill="#f43f5e" />
          </svg>

          <div className="absolute flex items-end gap-1.5 pointer-events-none" style={{ top: '35%', left: '18%', height: 50 }}>
            {[{ h: '40%', color: '#0ea5e9', delay: '0s' }, { h: '70%', color: '#34d399', delay: '0.3s' }, { h: '100%', color: '#34d399', delay: '0.6s' }].map((bar, i) => (
              <div key={i} className="rounded" style={{ width: 6, height: bar.h, background: bar.color, animation: 'pulseBar 2s ease-in-out infinite alternate', animationDelay: bar.delay }} />
            ))}
          </div>

          <div className="absolute flex items-end gap-1 pointer-events-none" style={{ bottom: '30%', right: '20%', height: 40 }}>
            {[{ h: '80%', color: '#0ea5e9', delay: '0.2s' }, { h: '50%', color: '#f43f5e', delay: '0.5s' }, { h: '30%', color: '#f43f5e', delay: '0.8s' }].map((bar, i) => (
              <div key={i} className="rounded" style={{ width: 5, height: bar.h, background: bar.color, animation: 'pulseBar 2.5s ease-in-out infinite alternate', animationDelay: bar.delay }} />
            ))}
          </div>

          <h1 className="relative z-10 text-white font-black tracking-tight select-none pointer-events-none text-6xl"
            style={{ fontFamily: 'Montserrat, sans-serif', textShadow: '0 10px 30px rgba(0,0,0,0.8)', lineHeight: 1 }}>
            Data<span className="text-emerald-400" style={{ textShadow: '0 0 25px rgba(52,211,153,0.5)' }}>Analysis</span>
          </h1>
        </div>

        {/* Kavisli Geçiş */}
        <div className="hidden md:block relative z-10 flex-shrink-0 bg-slate-950" style={{ width: 80 }}>
          <svg viewBox="0 0 100 100" preserveAspectRatio="none" className="w-full h-full block">
            <path d="M0,100 C50,100 50,0 100,0 L100,100 Z" fill="#ffffff" />
            <path d="M0,100 C50,100 50,0 100,0" fill="none" stroke="#34d399" strokeWidth="1.5"
              style={{ filter: 'drop-shadow(0 0 6px rgba(52,211,153,0.8))' }} />
          </svg>
        </div>

        {/* Sağ — İçerik */}
        <div className="bg-white flex flex-col justify-center flex-1">
          <div className="w-full mx-auto px-12 py-12 md:py-0" style={{ maxWidth: 500 }}>

            {/* Mobil Brand */}
            <div className="flex md:hidden items-center gap-3 mb-10">
              <div className="w-10 h-10 rounded-xl flex items-center justify-center flex-shrink-0 bg-emerald-400">
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="#fff" strokeWidth="2.5">
                  <path d="M12 2L2 7l10 5 10-5-10-5zM2 17l10 5 10-5M2 12l10 5 10-5" />
                </svg>
              </div>
              <span className="text-2xl font-black text-slate-900" style={{ fontFamily: 'Montserrat, sans-serif' }}>
                Data<span className="text-emerald-400">Analysis</span>
              </span>
            </div>

            {error && (
              <div className="flex items-start gap-3 bg-red-50 border border-red-200 text-red-700 rounded-2xl px-4 py-3.5 mb-6">
                <svg className="flex-shrink-0 mt-0.5" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                  <circle cx="12" cy="12" r="10" />
                  <line x1="12" y1="8" x2="12" y2="12" />
                  <line x1="12" y1="16" x2="12.01" y2="16" />
                </svg>
                <span className="text-sm font-medium">{error}</span>
              </div>
            )}

            {children}
          </div>
        </div>
      </div>
    </div>
  )
}