import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

import { AppContactsModule } from './app-contacts/app-contacts.module';
import { CafeContactsComponent } from './cafe-contacts.component';
import { ClientContactsModule } from './client-contacts/client-contacts.module';
import { ContactRolesService } from './common/services/contact-roles.service';
import { GenericContactsModule } from './generic-contact/generic-contacts.module';
import { SpecializedContactsModule } from './specialized-contacts/specialized-contacts.module';

@NgModule({
  declarations: [CafeContactsComponent],
  imports: [
    CommonModule,
    AppContactsModule,
    ClientContactsModule,
    GenericContactsModule,
    SpecializedContactsModule,
  ],
  providers: [ContactRolesService],
  exports: [CafeContactsComponent],
})
export class CafeContactsModule { }
