import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';

import { CafeComponent } from './cafe.component';
import { CafeFiltersModule } from './common/cafe-filters/cafe-filters.module';
import { CafeCategoryService } from './common/services/cafe-category.service';
import { CafeContactsModule } from './contacts/cafe-contacts.module';
import { CafeEnvironmentsModule } from './environments/cafe-environments.module';

@NgModule({
  declarations: [
    CafeComponent,
  ],
  imports: [
    CommonModule,
    TranslateModule,
    CafeFiltersModule,
    CafeEnvironmentsModule,
    CafeContactsModule,
    ReactiveFormsModule,
  ],
  providers: [CafeCategoryService],
})
export class CafeModule { }
