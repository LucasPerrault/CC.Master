import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@cc/aspects/translate';

import { NavigationComponent } from './navigation.component';
import { NavigationAlertService } from './services/navigation-alert.service';
import { NavigationTabsService } from './services/navigation-tabs.service';
import { ZendeskHelpService } from './services/zendesk-help.service';

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
    ZendeskHelpService,
    NavigationTabsService,
    NavigationAlertService,
  ],
})
export class NavigationModule { }
