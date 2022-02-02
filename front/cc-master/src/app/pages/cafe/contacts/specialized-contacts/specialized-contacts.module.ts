import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { PagingModule } from '@cc/common/paging';
import { LuTooltipTriggerModule } from '@lucca-front/ng/tooltip';

import { CafePageFilterTemplateModule } from '../../common/components/cafe-page-filter-template/cafe-page-filter-template.module';
import { CafePageTemplateModule } from '../../common/components/cafe-page-template/cafe-page-template.module';
import { CategorySelectModule } from '../../common/forms';
import {
  ContactAdditionalColumnSelectModule,
} from '../common/components/contact-additional-column-select/contract-additional-column-select.module';
import { ContactCategorySelectModule } from '../common/components/contact-category-select/contact-category-select.module';
import { SpecializedContactAdvancedFilterConfiguration } from './advanced-filter/specialized-contact-advanced-filter.configuration';
import {
  SpecializedContactAdvancedFilterApiMappingService,
} from './advanced-filter/specialized-contact-advanced-filter-api-mapping.service';
import { SpecializedContactFormlyConfiguration } from './advanced-filter/specialized-contact-formly.configuration';
import { SpecializedContactListComponent } from './specialized-contact-list/specialized-contact-list.component';
import { SpecializedContactsComponent } from './specialized-contacts.component';
import { SpecializedContactsDataService } from './specialized-contacts-data.service';

@NgModule({
  declarations: [
    SpecializedContactsComponent,
    SpecializedContactListComponent,
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
    SpecializedContactsDataService,
    SpecializedContactAdvancedFilterConfiguration,
    SpecializedContactAdvancedFilterApiMappingService,
    SpecializedContactFormlyConfiguration,
  ],
  exports: [SpecializedContactsComponent],
})
export class SpecializedContactsModule { }
