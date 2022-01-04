import { Component, EventEmitter, Input, Output } from '@angular/core';

import { ISyncRevenueInfo } from '../../models/sync-revenue-info.interface';
import { CurrentSyncRevenueInfo } from '../../services/sync-revenue.service';
import { BillingEntity, getBillingEntity } from '@cc/domain/billing/clients';
import { TranslatePipe } from '@cc/aspects/translate';

@Component({
	selector: 'cc-accounting-sync-revenue-card',
	templateUrl: './accounting-sync-revenue-card.component.html',
})
export class AccountingSyncRevenueCardComponent {
	@Input() public syncRevenueInfo: CurrentSyncRevenueInfo;
	@Input() public isLoading: boolean;
	@Input() public syncButtonState: string;
	@Output() public syncRevenue: EventEmitter<void> = new EventEmitter();

	public get isDisabled(): boolean {
		return this.isLoading || this.syncRevenueInfo.syncRevenue.lineCount === 0;
	}

	constructor(private translatePipe: TranslatePipe) { }

	public synchronise(): void {
		this.syncRevenue.emit();
	}

	public getBillingEntityName(billingEntity: BillingEntity): string {
		const translationKey = getBillingEntity(billingEntity)?.name;
		return this.translatePipe.transform(translationKey);
	}
}
