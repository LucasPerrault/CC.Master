import { ComparisonOperator } from '../enums/comparison-operator.enum';
import { LogicalOperator } from '../enums/logical-operator.enum';
import { ComparisonOperatorDto, ItemsMatchedDto } from './advanced-filter.interface';

interface IListComparisonOperator {
  operator: ComparisonOperatorDto;
  itemsMatched: ItemsMatchedDto;
  logicalOperator: LogicalOperator;
}

export class AdvancedFilterOperatorMapping {

  public static getComparisonOperatorDto(operator: ComparisonOperator): ComparisonOperatorDto {
    switch (operator) {
      case ComparisonOperator.Equals :
        return ComparisonOperatorDto.Equals;
      case ComparisonOperator.NotEquals :
        return ComparisonOperatorDto.NotEquals;
      case ComparisonOperator.TrueOnly :
        return ComparisonOperatorDto.Equals;
      case ComparisonOperator.FalseOnly :
        return ComparisonOperatorDto.Equals;
      case ComparisonOperator.StrictlyLessThan :
        return ComparisonOperatorDto.StrictlyLessThan;
      case ComparisonOperator.StrictlyGreaterThan:
        return ComparisonOperatorDto.StrictlyGreaterThan;
    }
  }

  public static getListComparisonOperatorDto(operator: ComparisonOperator): IListComparisonOperator {
      switch (operator) {
        case ComparisonOperator.ListContains:
          return {
            operator: ComparisonOperatorDto.Equals,
            itemsMatched: ItemsMatchedDto.Any,
            logicalOperator: LogicalOperator.And,
          };
        case ComparisonOperator.ListNotContains:
          return {
            operator: ComparisonOperatorDto.NotEquals,
            itemsMatched: ItemsMatchedDto.All,
            logicalOperator: LogicalOperator.And,
          };
        case ComparisonOperator.ListAreAmong:
          return {
            operator: ComparisonOperatorDto.Equals,
            itemsMatched: ItemsMatchedDto.Any,
            logicalOperator: LogicalOperator.Or,
          };
        case ComparisonOperator.ListContainsOnly:
          return {
            operator: ComparisonOperatorDto.Equals,
            itemsMatched: ItemsMatchedDto.All,
            logicalOperator: LogicalOperator.And,
          };
      }
  }
}
