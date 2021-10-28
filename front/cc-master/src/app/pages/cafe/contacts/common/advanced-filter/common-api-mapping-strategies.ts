import {
  AdvancedFilter,
  AdvancedFilterTypeMapping,
  ComparisonOperator,
  getComparisonBooleanValue,
} from '../../../common/cafe-filters/advanced-filter-form';
import { AdvancedFilterOperatorMapping } from '../../../common/cafe-filters/advanced-filter-form';

export class CommonApiMappingStrategies {

    public static getIsConfirmedAdvancedFilter(comparisonOperator: ComparisonOperator): AdvancedFilter {
      const isConfirmed = getComparisonBooleanValue(comparisonOperator);
      const operator = AdvancedFilterOperatorMapping.getComparisonOperatorDto(comparisonOperator);
      const toFilterCriterion = c => ({ isConfirmed: c });

      return AdvancedFilterTypeMapping.toAdvancedFilter([isConfirmed], operator, toFilterCriterion);
    }
}
