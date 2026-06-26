import { create } from 'zustand'

const useAuthStore = create((set) => ({
  user: null,
  isAuthenticated: false,

  setAuthenticated: (value) => set({ isAuthenticated: value }),
  setUser: (user) => set({ user }),

  logout: () => set({ user: null, isAuthenticated: false }),
}))

export default useAuthStore