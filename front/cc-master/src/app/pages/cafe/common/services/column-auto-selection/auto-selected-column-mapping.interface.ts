import { IAdvancedCriterionForm, IFacetComparisonCriterion } from '../../components/advanced-filter-form';
import { FacetAndColumnHelper, IFacetAndColumn } from '../../forms/select/facets-and-columns-api-select';
import { IAdditionalColumn } from '../../models';
import { IAutoSelectedCriterionForm } from './auto-selected-criterion.interface';

export interface IAutoSelectedColumnMapping {
  criterionKey: string;
  getColumn?(criterion?: IAutoSelectedCriterionForm): IFacetAndColumn;
}

export enum IAutoSelectedColumnState {
  Idle = 'Idle',
  WasAdded = 'WasAdded',
  WasRemoved = 'WasRemoved',
}

export class AutoSelectedColumnMappingHelper {
  public static getAdditionalColumn<TAdditionalColumnEnum>(
    id: TAdditionalColumnEnum,
    get: (id: TAdditionalColumnEnum) => IAdditionalColumn,
  ): IFacetAndColumn {
    return FacetAndColumnHelper.transformColumnToFacetAndColumn(get(id));
  }

  public static getFacetColumn(criterion?: IAutoSelectedCriterionForm): IFacetAndColumn {
    if (!criterion.content) {
      return;
    }

    const child = criterion?.content as IAdvancedCriterionForm;
    const facetCriterion = child?.criterion as IFacetComparisonCriterion;
    return FacetAndColumnHelper.transformFacetToFacetAndColumn(facetCriterion.facet);
  }
}
