import { Component, OnInit } from '@angular/core';
import { Operation, RightsService } from '@cc/aspects/rights';
import { Observable } from 'rxjs';

import { IContractDraftListEntry, IContractsDraftFilter } from './models';
import { ContractsDraftListService } from './services/contracts-draft-list.service';

@Component({
  selector: 'cc-contracts-draft',
  templateUrl: './contracts-draft.component.html',
})
export class ContractsDraftComponent implements OnInit {

  public filters: IContractsDraftFilter;

  public get drafts$(): Observable<IContractDraftListEntry[]> {
    return this.draftListService.drafts$;
  }

  public get isLoading$(): Observable<boolean> {
    return this.draftListService.isLoading$;
  }

  public get canCreateContracts(): boolean {
    return this.rightsService.hasOperation(Operation.CreateContracts);
  }

  constructor(private draftListService: ContractsDraftListService, private rightsService: RightsService) {}

  public ngOnInit(): void {
    this.draftListService.update();
  }

  public updateFilters(filters: IContractsDraftFilter): void {
    this.draftListService.update(filters);
  }
}
