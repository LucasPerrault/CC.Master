import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { startOfMonth } from 'date-fns';

import { getCloseContractReason } from '../../constants/close-contract-reason.enum';
import { ICloseContractMinDateReason } from '../../models/close-contract-min-date-reason.interface';
import { IContractClosureDetailed } from '../../models/contract-closure-detailed.interface';
import { IContractClosureForm } from '../../models/contract-closure-form.interface';

enum CloseContractFormKey {
  CloseReason = 'closeReason',
  CloseOn = 'closeOn'
}

@Component({
  selector: 'cc-contract-closure-form',
  templateUrl: './contract-closure-form.component.html',
  styleUrls: ['./contract-closure-form.component.scss'],
})
export class ContractClosureFormComponent {
  @Input() minDateReason: ICloseContractMinDateReason;
  @Input() closeButtonState: string;
  @Input() set contract(closureDetailed: IContractClosureDetailed) { this.setClosureForm(closureDetailed); }
  @Output() closeContract: EventEmitter<IContractClosureForm> = new EventEmitter<IContractClosureForm>();

  public get min(): Date {
    return !!this.minDateReason?.date ? startOfMonth(this.minDateReason.date) : null;
  }

  public formGroup: FormGroup;
  public formGroupKey = CloseContractFormKey;

  public showConfirmation = false;

  constructor() {
    this.formGroup = new FormGroup({
      [CloseContractFormKey.CloseReason]: new FormControl(),
      [CloseContractFormKey.CloseOn]: new FormControl(),
    });
  }

  public onCloseContract(): void {
    this.closeContract.emit(this.formGroup.value);
  }

  public setShowConfirmation(isShown: boolean): void {
    this.showConfirmation = isShown;
  }

  private setClosureForm(closureDetailed: IContractClosureDetailed): void {
    this.formGroup.patchValue({
      [CloseContractFormKey.CloseReason]: getCloseContractReason(closureDetailed.closeReason),
      [CloseContractFormKey.CloseOn]: closureDetailed.closeOn,
    });
  }
}
