import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { LuApiSearcherModule } from '@lucca-front/ng/api';
import { LuInputClearerModule, LuInputDisplayerModule } from '@lucca-front/ng/input';
import { LuForOptionsModule, LuOptionItemModule, LuOptionPagerModule, LuOptionPickerModule } from '@lucca-front/ng/option';
import { LuSelectInputModule } from '@lucca-front/ng/select';

import { PasswordInputComponent } from './password-input.component';

@NgModule({
  declarations: [PasswordInputComponent],
  imports: [
    CommonModule,
    LuSelectInputModule,
    FormsModule,
    TranslateModule,
    LuInputDisplayerModule,
    LuOptionPickerModule,
    LuApiSearcherModule,
    LuOptionPagerModule,
    LuOptionItemModule,
    LuInputClearerModule,
    LuForOptionsModule,
    ReactiveFormsModule,
  ],
  exports: [PasswordInputComponent],
})
export class PasswordInputModule { }
