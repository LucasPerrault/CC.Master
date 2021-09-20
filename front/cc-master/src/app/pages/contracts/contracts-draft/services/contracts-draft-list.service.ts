import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { finalize } from 'rxjs/operators';

import { IContractDraftListEntry, IContractsDraftFilter } from '../models';
import { ContractsDraftService } from './contracts-draft.service';
import { ContractsDraftApiMappingService } from './contracts-draft-api-mapping.service';

@Injectable()
export class ContractsDraftListService {
  private draftList: BehaviorSubject<IContractDraftListEntry[]> = new BehaviorSubject<IContractDraftListEntry[]>([]);
  private isLoading: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  public get drafts$(): Observable<IContractDraftListEntry[]> {
    return this.draftList.asObservable();
  }

  public get isLoading$(): Observable<boolean> {
    return this.isLoading.asObservable();
  }

  constructor(
    private draftsService: ContractsDraftService,
    private draftApiMappingService: ContractsDraftApiMappingService,
  ) {}

  public update(filters?: IContractsDraftFilter): void {
    this.isLoading.next(true);

    const httpParams = this.draftApiMappingService.toHttpParams(filters);

    this.draftsService.getContractDrafts$(httpParams)
      .pipe(finalize(() => this.isLoading.next(false)))
      .subscribe(data => this.draftList.next(data.items));
  }
}
