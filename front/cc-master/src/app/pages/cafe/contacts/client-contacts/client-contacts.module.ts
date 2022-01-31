import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { PagingModule } from '@cc/common/paging';
import { LuTooltipTriggerModule } from '@lucca-front/ng/tooltip';

import { CategorySelectModule } from '../../common/cafe-filters/category-filter/category-select/category-select.module';
import { CafePageFilterTemplateModule } from '../../common/components/cafe-page-filter-template/cafe-page-filter-template.module';
import { CafePageTemplateModule } from '../../common/components/cafe-page-template/cafe-page-template.module';
import {
  ContactAdditionalColumnSelectModule,
} from '../common/components/contact-additional-column-select/contract-additional-column-select.module';
import { ContactCategorySelectModule } from '../common/components/contact-category-select/contact-category-select.module';
import { ClientContactAdvancedFilterConfiguration } from './advanced-filter/client-contact-advanced-filter.configuration';
import { ClientContactAdvancedFilterApiMappingService } from './advanced-filter/client-contact-advanced-filter-api-mapping.service';
import { ClientContactFormlyConfiguration } from './advanced-filter/client-contact-formly-configuration.service';
import { ClientContactListComponent } from './client-contact-list/client-contact-list.component';
import { ClientContactsComponent } from './client-contacts.component';
import { ClientContactsDataService } from './client-contacts-data.service';

@NgModule({
  declarations: [
    ClientContactsComponent,
    ClientContactListComponent,
  ],
  imports: [
    CommonModule,
    TranslateModule,
    ContactAdditionalColumnSelectModule,
    ReactiveFormsModule,
    LuTooltipTriggerModule,
    PagingModule,
    CafePageTemplateModule,
    CafePageFilterTemplateModule,
    CategorySelectModule,
    ContactCategorySelectModule,
  ],
  providers: [
    ClientContactsDataService,
    ClientContactAdvancedFilterConfiguration,
    ClientContactFormlyConfiguration,
    ClientContactAdvancedFilterApiMappingService,
  ],
  exports: [ClientContactsComponent],
})
export class ClientContactsModule { }
