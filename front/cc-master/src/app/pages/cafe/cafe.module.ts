import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';

import { CafeComponent } from './cafe.component';
import { CafeConfiguration } from './cafe-configuration';
import { CafeFiltersModule } from './common/cafe-filters/cafe-filters.module';
import { CafeContactsModule } from './contacts/cafe-contacts.module';
import { CafeInstancesModule } from './instance/cafe-instances.module';

@NgModule({
  declarations: [
    CafeComponent,
  ],
  imports: [
    CommonModule,
    TranslateModule,
    CafeFiltersModule,
    CafeInstancesModule,
    CafeContactsModule,
    ReactiveFormsModule,
  ],
  providers: [CafeConfiguration],
})
export class CafeModule { }
