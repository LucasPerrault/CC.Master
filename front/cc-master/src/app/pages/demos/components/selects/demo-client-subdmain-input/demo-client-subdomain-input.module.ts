import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { LuApiSearcherModule } from '@lucca-front/ng/api';
import { LuInputClearerModule, LuInputDisplayerModule } from '@lucca-front/ng/input';
import { LuForOptionsModule, LuOptionItemModule, LuOptionPickerModule } from '@lucca-front/ng/option';
import { LuSelectInputModule } from '@lucca-front/ng/select';

import { DemoClientSubdomainInputComponent } from './demo-client-subdomain-input.component';
import { SubdomainAvailabilityDataService } from './subdomain-availability-data.service';

@NgModule({
  declarations: [DemoClientSubdomainInputComponent],
  exports: [DemoClientSubdomainInputComponent],
  imports: [
    CommonModule,
    LuSelectInputModule,
    ReactiveFormsModule,
    LuInputDisplayerModule,
    LuOptionPickerModule,
    LuApiSearcherModule,
    LuOptionItemModule,
    LuForOptionsModule,
    LuInputClearerModule,
    TranslateModule,
  ],
  providers: [SubdomainAvailabilityDataService],
})
export class DemoClientSubdomainInputModule {}
