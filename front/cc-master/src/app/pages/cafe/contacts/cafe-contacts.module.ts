import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

import { AppContactsModule } from './app-contacts/app-contacts.module';
import { CafeContactConfiguration } from './cafe-contact-configuration.service';
import { CafeContactsComponent } from './cafe-contacts.component';
import { ClientContactsModule } from './client-contacts/client-contacts.module';
import { SpecializedContactsModule } from './specialized-contacts/specialized-contacts.module';

@NgModule({
  declarations: [CafeContactsComponent],
  imports: [
    CommonModule,
    AppContactsModule,
    ClientContactsModule,
    SpecializedContactsModule,
  ],
  providers: [CafeContactConfiguration],
  exports: [CafeContactsComponent],
})
export class CafeContactsModule { }
