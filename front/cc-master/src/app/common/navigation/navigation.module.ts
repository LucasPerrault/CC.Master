import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@cc/aspects/translate';

import { getNavigationTabs } from './constants/navigation-tabs.const';
import { NavigationComponent } from './navigation.component';
import { NAVIGATION_TABS } from './navigation-tabs.token';
import { NavigationAlertService } from './services/navigation-alert.service';
import { NavigationTabsService } from './services/navigation-tabs.service';

@NgModule({
  declarations: [NavigationComponent],
  imports: [
    CommonModule,
    RouterModule,
    TranslateModule,
  ],
  exports: [
    NavigationComponent,
  ],
  providers: [
    NavigationTabsService,
    NavigationAlertService,
    {
      provide: NAVIGATION_TABS,
      useFactory: getNavigationTabs,
      deps: [NavigationAlertService],
    },
  ],
})
export class NavigationModule { }
