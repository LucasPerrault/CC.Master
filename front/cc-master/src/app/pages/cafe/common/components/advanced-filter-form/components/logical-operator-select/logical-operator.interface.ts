import { LogicalOperator } from '../../enums/logical-operator.enum';

export interface ILogicalOperator {
  id: LogicalOperator;
  name: string;
}

export const logicalOperators: ILogicalOperator[] = [
  {
    id: LogicalOperator.And,
    name: 'cafe_logicOperator_and',
  },
  {
    id: LogicalOperator.Or,
    name: 'cafe_logicOperator_or',
  },
];

export const getLogicalOperator = (id: LogicalOperator): ILogicalOperator =>
  logicalOperators.find(l => l.id === id);
