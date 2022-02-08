import { AdvancedFilter } from '../cafe-filters/advanced-filter-form';

export interface ISearchDto {
  criterion: AdvancedFilter;
}

export const toSearchDto = (criterion: AdvancedFilter): ISearchDto => ({ criterion });
