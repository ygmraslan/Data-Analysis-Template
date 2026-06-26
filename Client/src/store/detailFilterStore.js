import { create } from 'zustand'
import { EMPTY_FILTER } from '../utils/detailFilter'

const useDetailFilterStore = create((set) => ({
  productGroup: 'KASKO',
  filter: EMPTY_FILTER,
  
  setProductGroup: (pg) => set((state) => ({
    productGroup: pg,
    filter: { ...state.filter, productCodes: [] },
  })),

  setFilter: (next) => set((state) => ({
    filter: typeof next === 'function' ? next(state.filter) : next,
  })),

  resetFilter: () => set({ filter: EMPTY_FILTER }),
}))

export default useDetailFilterStore