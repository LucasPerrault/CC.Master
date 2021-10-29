import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { LuInputDisplayerModule, LuInputModule } from '@lucca-front/ng/input';
import {
  LuForOptionsModule,
  LuOptionFeederModule,
  LuOptionItemModule,
  LuOptionPickerModule,
  LuOptionSearcherModule,
} from '@lucca-front/ng/option';
import { LuSelectInputModule } from '@lucca-front/ng/select';

import { CategorySelectComponent } from './category-select.component';

@NgModule({
  declarations: [CategorySelectComponent],
  imports: [
    CommonModule,
    LuSelectInputModule,
    ReactiveFormsModule,
    LuInputDisplayerModule,
    LuOptionPickerModule,
    LuOptionFeederModule,
    LuOptionSearcherModule,
    LuOptionItemModule,
    LuInputModule,
    TranslateModule,
    LuForOptionsModule,
  ],
  exports: [CategorySelectComponent],
})
export class CategorySelectModule { }
