import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@cc/aspects/translate';
import { PriceListModule } from '@cc/domain/billing/offers';

import { PriceGridModalComponent } from './price-grid-modal.component';
import { PriceGridModalDataService } from './price-grid-modal-data.service';

@NgModule({
  declarations: [PriceGridModalComponent],
    imports: [
        CommonModule,
        TranslateModule,
        PriceListModule,
    ],
  exports: [PriceGridModalComponent],
  providers: [PriceGridModalDataService],
})
export class PriceGridModalModule { }
