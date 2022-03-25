import { Component, Inject, Input } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { BILLING_CORE_DATA, getNameByCode, IBillingCoreData } from '@cc/domain/billing/billling-core-data';
import { LuModal } from '@lucca-front/ng/modal';

import { IContractDraftListEntry } from '../../models';
import { ContractsDraftDeletionModalComponent } from '../contracts-draft-deletion-modal/contracts-draft-deletion-modal.component';

@Component({
  selector: 'cc-contracts-draft-list',
  templateUrl: './contracts-draft-list.component.html',
  styleUrls: ['./contracts-draft-list.component.scss'],
})
export class ContractsDraftListComponent {
  @Input() public canCreateContracts: boolean;
  @Input() public contractDrafts: IContractDraftListEntry[];
  @Input() public isLoading: boolean;

  constructor(
    private modal: LuModal,
    private translatePipe: TranslatePipe,
    @Inject(BILLING_CORE_DATA) private billingCoreData: IBillingCoreData,
    ) { }

  public getBillingEntityName(code: string): string {
    return getNameByCode(this.billingCoreData.billingEntities, code);
  }

  public redirectTo(externalUrl: string): void {
    window.open(externalUrl);
  }

  public get isEmpty(): boolean {
    return !this.contractDrafts.length && !this.isLoading;
  }

  public getContractCreationUrl(draftId: string): string[] {
    return this.canCreateContracts ? [draftId, 'form'] : [];
  }

  public remove(draft: IContractDraftListEntry): void {
    if (!draft || !this.canCreateContracts) {
      return;
    }

    this.modal.open(ContractsDraftDeletionModalComponent, draft);
  }
}
