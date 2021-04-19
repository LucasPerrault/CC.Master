import { Operation, OperationRestrictionMode } from '@cc/aspects/rights';
import { Observable } from 'rxjs';

export interface INavigationTab {
  name: string;
  icon?: string;
  url: string;
  isLegacy?: boolean;
  children?: INavigationTab[];
  alert$?: Observable<string>;
  restriction?: INavigationRestriction;
}

interface INavigationRestriction {
  operations?: Operation[];
  mode?: OperationRestrictionMode;
}

