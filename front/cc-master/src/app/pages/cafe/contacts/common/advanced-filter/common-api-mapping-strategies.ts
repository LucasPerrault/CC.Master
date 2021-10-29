import {
    AdvancedFilter,
    AdvancedFilterTypeMapping,
    ComparisonOperator,
    getComparisonBooleanValue,
} from '../../../common/cafe-filters/advanced-filter-form';

export class CommonApiMappingStrategies {

    public static getIsConfirmedAdvancedFilter(operator: ComparisonOperator): AdvancedFilter {
      const isConfirmed = getComparisonBooleanValue(operator);
      return AdvancedFilterTypeMapping.toAdvancedFilter([isConfirmed], ComparisonOperator.Equals, c => ({ isConfirmed: c }));
    }
}
