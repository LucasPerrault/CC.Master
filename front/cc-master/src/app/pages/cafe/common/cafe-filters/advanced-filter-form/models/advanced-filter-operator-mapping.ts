import { ComparisonOperator } from '../enums/comparison-operator.enum';
import { LogicalOperator } from '../enums/logical-operator.enum';
import { ComparisonOperatorDto, ItemsMatchedDto } from './advanced-filter.interface';

interface IListComparisonOperator {
  operator: ComparisonOperatorDto;
  itemsMatched: ItemsMatchedDto;
  logicalOperator: LogicalOperator;
}
interface IElementOperator {
  operator: ComparisonOperatorDto;
  logicalOperator: LogicalOperator;
}

export class AdvancedFilterOperatorMapping {

  public static getComparisonOperatorDto(operator: ComparisonOperator): IElementOperator {
    switch (operator) {
      case ComparisonOperator.Equals :
        return { operator: ComparisonOperatorDto.Equals, logicalOperator: LogicalOperator.Or };
      case ComparisonOperator.NotEquals :
        return { operator: ComparisonOperatorDto.NotEquals, logicalOperator: LogicalOperator.And };
      case ComparisonOperator.TrueOnly :
        return { operator: ComparisonOperatorDto.Equals, logicalOperator: LogicalOperator.Or };
      case ComparisonOperator.FalseOnly :
        return { operator: ComparisonOperatorDto.Equals, logicalOperator: LogicalOperator.Or };
      case ComparisonOperator.StrictlyLessThan :
        return { operator: ComparisonOperatorDto.StrictlyLessThan, logicalOperator: LogicalOperator.And };
      case ComparisonOperator.StrictlyGreaterThan:
        return { operator: ComparisonOperatorDto.StrictlyGreaterThan, logicalOperator: LogicalOperator.And };
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
