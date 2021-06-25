import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { LuInputClearerModule, LuInputDisplayerModule } from '@lucca-front/ng/input';
import { LuForOptionsModule, LuOptionFeederModule, LuOptionItemModule, LuOptionPickerModule } from '@lucca-front/ng/option';
import { LuSelectInputModule } from '@lucca-front/ng/select';
import { TranslateModule } from '@ngx-translate/core';

import { LifecycleSelectComponent } from './lifecycle-select.component';

@NgModule({
  declarations: [LifecycleSelectComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    LuSelectInputModule,
    LuOptionPickerModule,
    LuInputDisplayerModule,
    LuOptionItemModule,
    LuInputClearerModule,
    TranslateModule,
    LuOptionFeederModule,
    LuForOptionsModule,
  ],
  exports: [LifecycleSelectComponent],
})
export class LifecycleSelectModule { }
