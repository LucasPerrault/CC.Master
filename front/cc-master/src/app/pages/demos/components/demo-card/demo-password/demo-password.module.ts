import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { PasswordInputModule } from '@cc/common/forms/input/password/password-input.module';
import { LuTooltipTriggerModule } from '@lucca-front/ng/tooltip';

import { DemoInstanceUserApiSelectModule } from '../../selects';
import { DemoPasswordComponent } from './demo-password.component';

@NgModule({
  declarations: [DemoPasswordComponent],
  exports: [DemoPasswordComponent],
    imports: [
        CommonModule,
        DemoInstanceUserApiSelectModule,
        ReactiveFormsModule,
        LuTooltipTriggerModule,
        TranslateModule,
        PasswordInputModule,
    ],
})
export class DemoPasswordModule {}
