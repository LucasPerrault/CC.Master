import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@cc/aspects/translate';

import { PriceListComponent } from './price-list.component';

@NgModule({
  declarations: [PriceListComponent],
  imports: [
    CommonModule,
    TranslateModule,
  ],
  exports: [PriceListComponent],
})
export class PriceListModule { }
