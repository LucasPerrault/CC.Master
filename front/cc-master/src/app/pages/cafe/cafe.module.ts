import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';

import { CafeComponent } from './cafe.component';
import { CafeConfiguration } from './cafe-configuration';
import { CafeExportService } from './cafe-export.service';
import { CafeFiltersModule } from './common/cafe-filters/cafe-filters.module';
import { CafeContactsModule } from './contacts/cafe-contacts.module';
import { CafeEnvironmentsModule } from './environments/cafe-environments.module';
import { EstablishmentsModule } from './establishments/establishments.module';

@NgModule({
  declarations: [CafeComponent],
  imports: [
    CommonModule,
    TranslateModule,
    CafeFiltersModule,
    CafeEnvironmentsModule,
    CafeContactsModule,
    EstablishmentsModule,
    ReactiveFormsModule,
  ],
  providers: [CafeConfiguration, CafeExportService],
})
export class CafeModule { }
