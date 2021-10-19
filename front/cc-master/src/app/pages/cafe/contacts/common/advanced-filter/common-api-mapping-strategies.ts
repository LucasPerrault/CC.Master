import {
    AdvancedFilter,
    AdvancedFilterTypeMapping,
    ComparisonOperator,
    getComparisonBooleanValue,
} from '../../../common/cafe-filters/advanced-filter-form';

export class CommonApiMappingStrategies {

    public static getIsConfirmedAdvancedFilter(operator: ComparisonOperator): AdvancedFilter {
        const criterion = AdvancedFilterTypeMapping.toCriterion(
            ComparisonOperator.Equals,
            getComparisonBooleanValue(operator),
            c => ({ isConfirmed: c }),
        );
        return AdvancedFilterTypeMapping.toFilterCriterion(criterion);
    }
}
