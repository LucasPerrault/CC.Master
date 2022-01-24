import { IDistributor } from '@cc/domain/billing/distributors/v4';

import { IDemoAuthor } from './demo.interface';

export interface IDemoFilters {
  search: string;
  author: IDemoAuthor;
  distributor: IDistributor;
  isProtected: boolean;
}

export enum DemoFilterFormKey {
  Search = 'search',
  Author = 'author',
  Distributor = 'distributor',
  IsProtected = 'isProtected',
}
