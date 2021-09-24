import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@cc/aspects/translate';

import { CafeContactsComponent } from './cafe-contacts.component';
import { ContactListComponent } from './components/contact-list/contact-list.component';
import { AppContactsDataService } from './services/app-contacts-data.service';
import { ClientContactsDataService } from './services/client-contacts-data.service';
import { ContactRolesService } from './services/contact-roles.service';
import { ContactsListService } from './services/contacts-list.service';
import { SpecializedContactsDataService } from './services/specialized-contacts-data.service';
import { CommonContactsDataService } from './services/common-contacts-data.service';
import { LuTooltipTriggerModule } from '@lucca-front/ng/tooltip';
import {
  ContactAdditionalColumnSelectModule,
} from './components/contact-additional-column-select/contract-additional-column-select.module';
import { ReactiveFormsModule } from '@angular/forms';
import { PagingModule } from '@cc/common/paging';

@NgModule({
  declarations: [
    ContactListComponent,
    CafeContactsComponent,
  ],
  imports: [
    CommonModule,
    TranslateModule,
    LuTooltipTriggerModule,
    ContactAdditionalColumnSelectModule,
    ReactiveFormsModule,
    PagingModule,
  ],
  providers: [
    AppContactsDataService,
    SpecializedContactsDataService,
    ClientContactsDataService,
    ContactsListService,
    CommonContactsDataService,
    ContactRolesService,
  ],
  exports: [
    CafeContactsComponent,
  ],
})
export class CafeContactsModule { }
