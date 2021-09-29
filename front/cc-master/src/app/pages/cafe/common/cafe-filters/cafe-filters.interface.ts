import { IAdvancedFilterForm } from './advanced-filter-form/advanced-filter-form.interface';
import { ICategory } from './category-filter/category-select/category.interface';

export interface ICafeFilters {
  category: ICategory;
  advancedFilterForm: IAdvancedFilterForm;
}
