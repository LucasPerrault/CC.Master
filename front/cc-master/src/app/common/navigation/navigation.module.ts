import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@cc/aspects/translate';

import { getNavigationTabs } from './constants/navigation-tabs.const';
import { NAVIGATION_TABS } from './navigation-tabs.token';
import { NavigationAlertService } from './services/navigation-alert.service';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    RouterModule,
    TranslateModule,
  ],
  exports: [
  ],
  providers: [
    NavigationAlertService,
    {
      provide: NAVIGATION_TABS,
      useFactory: getNavigationTabs,
      deps: [NavigationAlertService],
    },
  ],
})
export class NavigationModule { }
