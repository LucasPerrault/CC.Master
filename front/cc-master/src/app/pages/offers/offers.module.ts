import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@cc/aspects/translate';

import { OffersComponent } from './offers.component';

@NgModule({
  declarations: [
    OffersComponent,
  ],
    imports: [
        CommonModule,
        TranslateModule,
    ],
})
export class OffersModule { }
