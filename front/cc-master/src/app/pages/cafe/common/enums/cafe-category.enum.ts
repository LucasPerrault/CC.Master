import { ICategory } from '../forms/select/category-select/category.interface';

export enum CafeCategory {
  Environments = 'environments',
  Contacts = 'contacts',
}

export const categories: ICategory[] = [
  {
    id: CafeCategory.Environments,
    name: 'cafe_category_environments',
  },
  {
    id: CafeCategory.Contacts,
    name: 'cafe_category_contacts',
  },
];

export const getCategory = (categoryId: CafeCategory) => categories.find(c => c.id === categoryId);

