import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { startOfMonth } from 'date-fns';

import { getCloseContractReason } from '../../constants/close-contract-reason.enum';
import { IClosureFormValidationContext } from '../../models/closure-form-validation-context.interface';
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
  @Input() formValidationContext: IClosureFormValidationContext;
  @Input() minContractClosedDate: Date;
  @Input() closeButtonState: string;
  @Input() set contractClosureDetailed(contractClosureDetailed: IContractClosureDetailed) {
    this.formGroup.patchValue({
      [CloseContractFormKey.CloseReason]: getCloseContractReason(contractClosureDetailed.closeReason),
      [CloseContractFormKey.CloseOn]: contractClosureDetailed.closeOn,
    });
  }
  @Output() closeContract: EventEmitter<IContractClosureForm> = new EventEmitter<IContractClosureForm>();

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

  public getFirstDay(date: Date): Date {
   return startOfMonth(date);
  }
}
