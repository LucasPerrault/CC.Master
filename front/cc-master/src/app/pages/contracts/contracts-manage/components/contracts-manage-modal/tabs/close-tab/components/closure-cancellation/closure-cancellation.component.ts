import { DatePipe } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { addMonths, startOfMonth } from 'date-fns';

import { CloseContractReason, closeContractReasons } from '../../constants/close-contract-reason.enum';
import { IContractClosureDetailed } from '../../models/contract-closure-detailed.interface';

@Component({
  selector: 'cc-closure-cancellation',
  templateUrl: './closure-cancellation.component.html',
})
export class ClosureCancellationComponent {
  @Input() contractClosureDetailed: IContractClosureDetailed;
  @Input() closeCancelButtonState: string;
  @Output() cancelClosure: EventEmitter<void> = new EventEmitter<void>();

  public showConfirmation = false;

  constructor(private translatePipe: TranslatePipe, private datePipe: DatePipe) { }

  public cancelContractClosure(): void {
    this.cancelClosure.emit();
  }

  public setShowConfirmation(isShown: boolean): void {
    this.showConfirmation = isShown;
  }

  public getContractClosedInformation(): string {
    return this.translatePipe.transform('front_contractPage_endContractCallout_text', {
      closeOn: this.datePipe.transform(new Date(this.contractClosureDetailed.closeOn), 'mediumDate'),
      closeReason: this.getTranslatedCloseReason(this.contractClosureDetailed.closeReason),
    });
  }

  public getClosureCancellationConfirmation(): string {
    return this.translatePipe.transform('front_contractPage_cancelContractClosure_confirmation', {
      date: this.getEstablishmentActivationDate(),
    });
  }

  private getTranslatedCloseReason(closeReason: CloseContractReason): string {
    const reason = closeContractReasons.find(r => r.id === closeReason);
    return this.translatePipe.transform(reason.name).toLowerCase();
  }

  private getEstablishmentActivationDate(): string {
    const closeOnAsDate = new Date(this.contractClosureDetailed.closeOn);
    const establishmentActivationDate = startOfMonth(addMonths(closeOnAsDate, 1));
    return this.datePipe.transform(establishmentActivationDate, 'MMMM YYYY');
  }
}
