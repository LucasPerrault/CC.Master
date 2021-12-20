import { Component } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { ALuPopupRef } from '@lucca-front/ng/popup';

@Component({
  selector: 'cc-contract-leaving-confirmation-popup',
  templateUrl: './contract-leaving-confirmation-popup.component.html',
})
export class ContractLeavingConfirmationPopupComponent {

  public get confirmLabel(): string {
    return this.translatePipe.transform('contracts_leaving_popup_confirm_button');
  }

  public get cancelLabel(): string {
    return this.translatePipe.transform('contracts_leaving_popup_cancel_button');
  }

  public get description(): string {
    return this.translatePipe.transform('contracts_leaving_popup_description');
  }

  constructor(
    private dialogRef: ALuPopupRef<ContractLeavingConfirmationPopupComponent>,
    private translatePipe: TranslatePipe,
  ) {}

  public confirm(): void {
    this.dialogRef.close(true);
  }

  public close(): void {
    this.dialogRef.close(false);
  }

  public cancel(): void {
    this.dialogRef.close(false);
  }
}
