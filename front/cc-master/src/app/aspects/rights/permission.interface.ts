import { Operation } from './operation.enum';

export interface IPermission {
  operation: IOperation;
}

interface IOperation {
  id: Operation;
}
