import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

import { CodeSourceSelectComponent } from './code-source-select.component';
import { LuSelectInputModule } from '@lucca-front/ng/select';
import { ReactiveFormsModule } from '@angular/forms';
import { LuOptionItemModule, LuOptionPickerModule } from '@lucca-front/ng/option';
import { LuInputClearerModule, LuInputDisplayerModule } from '@lucca-front/ng/input';
import { TranslateModule } from '@cc/aspects/translate';

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
