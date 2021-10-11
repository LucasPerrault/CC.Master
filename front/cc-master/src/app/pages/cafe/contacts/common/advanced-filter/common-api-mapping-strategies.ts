import { IEnvironment } from '@cc/domain/environments';

import {
    AdvancedFilter,
    AdvancedFilterTypeMapping,
    ComparisonOperator,
    getComparisonBooleanValue,
    LogicalOperator,
} from '../../../common/cafe-filters/advanced-filter-form';
import { IAppInstance } from '../../../environments/models/app-instance.interface';

export class CommonApiMappingStrategies {

    public static getEnvironmentAppInstancesAdvancedFilter(operator: ComparisonOperator, appInstances: IAppInstance[]): AdvancedFilter {
        const criterions = AdvancedFilterTypeMapping.toCriterions(
            operator,
            appInstances.map(a => a.id),
            c => ({ environment: { appInstance: { applicationId: c } } }),
        );
        return AdvancedFilterTypeMapping.toFilterCombination(LogicalOperator.And, criterions);
    }


    public static getSubdomainAdvancedFilter(operator: ComparisonOperator, environments: IEnvironment[]): AdvancedFilter {
        const criterions = AdvancedFilterTypeMapping.toCriterions(
            operator,
            environments.map(a => a.subDomain),
            c => ({ subdomain: c }),
        );
        return AdvancedFilterTypeMapping.toFilterCombination(LogicalOperator.And, criterions);
    }

    public static getIsConfirmedAdvancedFilter(operator: ComparisonOperator): AdvancedFilter {
        const criterion = AdvancedFilterTypeMapping.toCriterion(
            ComparisonOperator.Equals,
            getComparisonBooleanValue(operator),
            c => ({ isConfirmed: c }),
        );
        return AdvancedFilterTypeMapping.toFilterCriterion(criterion);
    }
}
