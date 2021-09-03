import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';

import { CafeComponent } from './cafe.component';

@NgModule({
  declarations: [
    CafeComponent,
  ],
  imports: [
    CommonModule,
    TranslateModule,
  ],
})
export class CafeModule { }
