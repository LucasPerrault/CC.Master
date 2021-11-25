import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@cc/aspects/translate';

import { ContractLeavingConfirmationPopupComponent } from './contract-leaving-confirmation-popup.component';

@NgModule({
  declarations: [ContractLeavingConfirmationPopupComponent],
  imports: [CommonModule, TranslateModule],
})
export class ContractLeavingConfirmationPopupModule { }
