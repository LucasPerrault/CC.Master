import { Component, Input } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { BillingEntity, getBillingEntity } from '@cc/domain/billing/clients';
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

  constructor(private modal: LuModal, private translatePipe: TranslatePipe) { }

  public getBillingEntityName(billingEntity: BillingEntity): string {
    const translationKey = getBillingEntity(billingEntity)?.name;
    return this.translatePipe.transform(translationKey);
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
