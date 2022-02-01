import {
  AdvancedFilter,
  AdvancedFilterTypeMapping,
  ComparisonOperator,
  getComparisonBooleanValue,
} from '../../../common/components/advanced-filter-form';
import { AdvancedFilterOperatorMapping } from '../../../common/components/advanced-filter-form';

export class CommonApiMappingStrategies {

    public static getIsConfirmedAdvancedFilter(comparisonOperator: ComparisonOperator): AdvancedFilter {
      const isConfirmed = getComparisonBooleanValue(comparisonOperator);
      const { operator, logicalOperator } = AdvancedFilterOperatorMapping.getComparisonOperatorDto(comparisonOperator);
      const toFilterCriterion = c => ({ isConfirmed: c });

      return AdvancedFilterTypeMapping.toAdvancedFilter([isConfirmed], operator, toFilterCriterion, logicalOperator);
    }
}
