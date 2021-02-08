import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { ALuDateAdapter, LuNativeDateAdapter } from '@lucca-front/ng/core';
import { LuDateModule } from '@lucca-front/ng/date';

import { DateRangeSelectComponent } from './date-range-select.component';

@NgModule({
  declarations: [DateRangeSelectComponent],
  imports: [
    LuDateModule,
    FormsModule,
    TranslateModule,
  ],
  exports: [DateRangeSelectComponent],
  providers: [
    LuNativeDateAdapter,
    { provide: ALuDateAdapter, useClass: LuNativeDateAdapter },
  ],
})
export class DateRangeSelectModule { }
