import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { LuInputClearerModule, LuInputDisplayerModule } from '@lucca-front/ng/input';
import { LuOptionItemModule, LuOptionPickerModule } from '@lucca-front/ng/option';
import { LuSelectInputModule } from '@lucca-front/ng/select';

import { CodeSourceSelectComponent } from './code-source-select.component';

@NgModule({
  declarations: [CodeSourceSelectComponent],
  imports: [
    CommonModule,
    LuSelectInputModule,
    ReactiveFormsModule,
    LuOptionPickerModule,
    LuOptionItemModule,
    LuInputClearerModule,
    LuInputDisplayerModule,
    TranslateModule,
  ],
  exports: [CodeSourceSelectComponent],
})
export class CodeSourceSelectModule { }
