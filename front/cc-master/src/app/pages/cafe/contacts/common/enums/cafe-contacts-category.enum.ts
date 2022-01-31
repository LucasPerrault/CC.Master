import { ICategory } from '../../../common/cafe-filters/category-filter/category-select/category.interface';

export enum ContactCategory {
  Specialized = 'specialized',
  Client = 'client',
  Application = 'application',
}

export const categories: ICategory[] = [
  {
    id: ContactCategory.Application,
    name: 'cafe_category_applicationContacts',
  },
  {
    id: ContactCategory.Client,
    name: 'cafe_category_clientContacts',
  },
  {
    id: ContactCategory.Specialized,
    name: 'cafe_category_specializedContacts',
  },
];

export const getCategory = (categoryId: ContactCategory) => categories.find(c => c.id === categoryId);
