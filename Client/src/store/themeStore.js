import { create } from 'zustand'

const useThemeStore = create((set) => ({
  isDark: false,

  toggleTheme: () => set((state) => {
    const next = !state.isDark
    if (next) {
      document.documentElement.classList.add('dark')
      localStorage.setItem('theme', 'dark')
    } else {
      document.documentElement.classList.remove('dark')
      localStorage.setItem('theme', 'light')
    }
    return { isDark: next }
  }),
}))

export default useThemeStore