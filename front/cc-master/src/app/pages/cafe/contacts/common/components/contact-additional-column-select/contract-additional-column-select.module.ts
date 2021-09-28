import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { LuInputClearerModule, LuInputDisplayerModule } from '@lucca-front/ng/input';
import { LuForOptionsModule, LuOptionFeederModule, LuOptionItemModule, LuOptionPickerModule } from '@lucca-front/ng/option';
import { LuSelectInputModule } from '@lucca-front/ng/select';

import { ContactAdditionalColumnSelectComponent } from './contact-additional-column-select.component';


@NgModule({
  declarations: [ContactAdditionalColumnSelectComponent],
  imports: [
    CommonModule,
    LuSelectInputModule,
    ReactiveFormsModule,
    LuInputDisplayerModule,
    LuOptionPickerModule,
    LuOptionFeederModule,
    LuOptionItemModule,
    LuForOptionsModule,
    LuInputClearerModule,
    TranslateModule,
  ],
  exports: [ContactAdditionalColumnSelectComponent],
})
export class ContactAdditionalColumnSelectModule { }
