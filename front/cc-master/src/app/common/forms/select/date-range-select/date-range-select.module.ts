import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { ALuDateAdapter, LuNativeDateAdapter } from '@lucca-front/ng/core';
import { LuDateModule } from '@lucca-front/ng/date';

import { DateRangeSelectComponent } from './date-range-select.component';

@NgModule({
  declarations: [DateRangeSelectComponent],
  imports: [
    LuDateModule,
    ReactiveFormsModule,
    TranslateModule,
  ],
  exports: [DateRangeSelectComponent],
  providers: [
    LuNativeDateAdapter,
    { provide: ALuDateAdapter, useClass: LuNativeDateAdapter },
  ],
})
export class DateRangeSelectModule { }
