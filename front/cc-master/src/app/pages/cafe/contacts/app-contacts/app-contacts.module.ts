import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { PagingModule } from '@cc/common/paging';
import { LuTooltipTriggerModule } from '@lucca-front/ng/tooltip';

import {
  ContactAdditionalColumnSelectModule,
} from '../common/components/contact-additional-column-select/contract-additional-column-select.module';
import { AppContactAdvancedFilterConfiguration } from './advanced-filter/app-contact-advanced-filter.configuration';
import { AppContactAdvancedFilterApiMappingService } from './advanced-filter/app-contact-advanced-filter-api-mapping.service';
import { AppContactFormlyConfiguration } from './advanced-filter/app-contact-formly.configuration';
import { AppContactListComponent } from './app-contact-list/app-contact-list.component';
import { AppContactsComponent } from './app-contacts.component';
import { AppContactsDataService } from './app-contacts-data.service';

@NgModule({
  declarations: [
    AppContactsComponent,
    AppContactListComponent,
  ],
  imports: [
    CommonModule,
    TranslateModule,
    ContactAdditionalColumnSelectModule,
    ReactiveFormsModule,
    LuTooltipTriggerModule,
    PagingModule,
  ],
  providers: [
    AppContactsDataService,
    AppContactFormlyConfiguration,
    AppContactAdvancedFilterConfiguration,
    AppContactAdvancedFilterApiMappingService,
  ],
  exports: [AppContactsComponent],
})
export class AppContactsModule { }
