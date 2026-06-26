import { HiSave } from 'react-icons/hi'

export default function SaveResultButton({ label, onClick }) {
  return (
    <div className="flex justify-center pt-4">
      <button
        onClick={onClick}
        className="flex items-center gap-2.5 px-8 py-3 text-sm font-semibold rounded-xl bg-gradient-to-r from-emerald-500 to-emerald-600 hover:from-emerald-600 hover:to-emerald-700 text-white shadow-lg shadow-emerald-500/25 hover:shadow-emerald-500/40 transition-all hover:-translate-y-0.5"
      >
        <HiSave className="w-5 h-5" />
        {label}
      </button>
    </div>
  )
}