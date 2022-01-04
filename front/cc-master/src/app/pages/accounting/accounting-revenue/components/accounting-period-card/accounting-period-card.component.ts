import { Component, EventEmitter, Input, Output } from '@angular/core';
import { endOfMonth, isBefore, startOfMonth } from 'date-fns';
import { CurrentAccountingPeriod } from '../../services/accounting-period.service';
import { BillingEntity, getBillingEntity } from '@cc/domain/billing/clients';
import { TranslatePipe } from '@cc/aspects/translate';

@Component({
  selector: 'cc-accounting-period-card',
  templateUrl: './accounting-period-card.component.html',
})
export class AccountingPeriodCardComponent {
  @Input() public accountingPeriod: CurrentAccountingPeriod;
  @Input() public isLoading: boolean;
  @Input() public closeButtonState: string;
  @Output() public closeCurrentPeriod: EventEmitter<Date> = new EventEmitter();

  public get canCloseCurrentPeriod(): boolean {
    const today = new Date();
    return isBefore(endOfMonth(this.accountingPeriod.date), startOfMonth(today));
  }

  constructor(private translatePipe: TranslatePipe) { }

  public close(): void {
    this.closeCurrentPeriod.emit(this.accountingPeriod.date);
	}
	
	public getBillingEntityName(billingEntity: BillingEntity): string {
		const translationKey = getBillingEntity(billingEntity)?.name;
		return this.translatePipe.transform(translationKey);
	}
}
