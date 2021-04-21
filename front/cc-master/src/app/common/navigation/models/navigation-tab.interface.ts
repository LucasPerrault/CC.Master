import { Operation, OperationRestrictionMode } from '@cc/aspects/rights';

import { NavigationAlert } from '../constants/navigation-alert.enum';

export interface INavigationTab {
  name: string;
  icon?: string;
  url: string;
  isLegacy?: boolean;
  children?: INavigationTab[];
  alert?: NavigationAlert;
  restriction?: INavigationRestriction;
}

interface INavigationRestriction {
  operations?: Operation[];
  mode?: OperationRestrictionMode;
}

