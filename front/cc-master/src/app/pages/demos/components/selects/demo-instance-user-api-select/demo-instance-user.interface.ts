import { IDemoAuthor } from '../../../models/demo.interface';

export interface IDemoInstanceUser extends IDemoAuthor {
  dtContractStart: string;
  dtContractEnd: string;
}
