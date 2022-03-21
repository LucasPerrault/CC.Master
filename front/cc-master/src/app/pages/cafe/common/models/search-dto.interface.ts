import { AdvancedFilter } from '../components/advanced-filter-form';
import { IFacet } from './facet.interface';

export interface ISearchDto {
  criterion: AdvancedFilter;
  facetIds?: number[];
}

export const toSearchDto = (criterion: AdvancedFilter, facets?: IFacet[]): ISearchDto => ({
  criterion,
  facetIds: facets?.map(f => f.id) ?? null,
});
