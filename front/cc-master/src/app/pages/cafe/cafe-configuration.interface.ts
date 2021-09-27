import { IAdvancedFilterConfiguration } from './common/cafe-filters/advanced-filters';
import { ICategory } from './common/cafe-filters/category-filter/category-select/category.interface';

export interface ICafeConfiguration {
  categories: ICategory[];
  advancedFilters: IAdvancedFilterConfiguration[];
}
