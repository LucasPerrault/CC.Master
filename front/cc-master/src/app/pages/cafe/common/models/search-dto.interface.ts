import { AdvancedFilter } from '../components/advanced-filter-form';
import { IFacet, IFacetIdentifier } from './facet.interface';

export interface ISearchDto {
  criterion: AdvancedFilter;
  facets?: IFacetIdentifier[];
}

export const toSearchDto = (criterion: AdvancedFilter, f?: IFacet[]): ISearchDto => ({
  criterion,
  facets: toFacetIdentifiers(f) ?? [],
});

const toFacetIdentifiers = (facets: IFacet[]): IFacetIdentifier[] => facets?.map(facet => ({
  code: facet.code,
  applicationId: facet.applicationId,
}));
