import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { PagingModule } from '@cc/common/paging';
import { LuTooltipTriggerModule } from '@lucca-front/ng/tooltip';

import {
  ContactAdditionalColumnSelectModule,
} from '../common/components/contact-additional-column-select/contract-additional-column-select.module';
import { GenericContactListComponent } from './generic-contact-list/generic-contact-list.component';
import { GenericContactsComponent } from './generic-contacts.component';
import { GenericContactsDataService } from './generic-contacts-data.service';

@NgModule({
  declarations: [
    GenericContactsComponent,
    GenericContactListComponent,
  ],
  imports: [
    CommonModule,
    TranslateModule,
    ContactAdditionalColumnSelectModule,
    ReactiveFormsModule,
    LuTooltipTriggerModule,
    PagingModule,
  ],
  providers: [GenericContactsDataService],
  exports: [GenericContactsComponent],
})
export class GenericContactsModule { }
