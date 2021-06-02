import { Operation } from './enums/operation.enum';

export interface IPermission {
  operation: IOperation;
}

interface IOperation {
  id: Operation;
}
