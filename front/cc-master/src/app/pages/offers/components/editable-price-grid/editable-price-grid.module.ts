import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { LuNumberModule } from '@lucca-front/ng/number';

import { OfferPriceListApiSelectModule } from '../offer-selects';
import { EditablePriceCellComponent } from './editable-price-cell/editable-price-cell.component';
import { EditablePriceGridComponent } from './editable-price-grid.component';



@NgModule({
  declarations: [EditablePriceGridComponent, EditablePriceCellComponent],
    imports: [
        CommonModule,
        TranslateModule,
        FormsModule,
        ReactiveFormsModule,
        OfferPriceListApiSelectModule,
        LuNumberModule,
    ],
  exports: [EditablePriceGridComponent],
})
export class EditablePriceGridModule { }
