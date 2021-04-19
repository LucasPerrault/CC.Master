import { InjectionToken } from '@angular/core';

import { INavigationTab } from './models/navigation-tab.interface';

export const NAVIGATION_TABS = new InjectionToken<INavigationTab[]>('navigation.tab');
