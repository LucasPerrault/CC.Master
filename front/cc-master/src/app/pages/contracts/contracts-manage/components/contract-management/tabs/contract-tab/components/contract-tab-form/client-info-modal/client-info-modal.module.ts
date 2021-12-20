import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@cc/aspects/translate';
import { LuTooltipModule } from '@lucca-front/ng/tooltip';

import { ClientInfoModalComponent } from './client-info-modal.component';

@NgModule({
  declarations: [
    ClientInfoModalComponent,
  ],
  imports: [
    CommonModule,
    TranslateModule,
    LuTooltipModule,
  ],
})
export class ClientInfoModalModule { }
