import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';

import { CafeComponent } from './cafe.component';
import { CafeFiltersModule } from './common/cafe-filters/cafe-filters.module';

@NgModule({
  declarations: [
    CafeComponent,
  ],
  imports: [
    CommonModule,
    TranslateModule,
    CafeFiltersModule,
  ],
})
export class CafeModule { }
