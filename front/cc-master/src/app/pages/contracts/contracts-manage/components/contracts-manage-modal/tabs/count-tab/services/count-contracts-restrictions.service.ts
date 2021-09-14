import { Injectable } from '@angular/core';
import { Operation, RightsService } from '@cc/aspects/rights';
import { CountCode } from '@cc/domain/billing/counts';

import { ICountListEntry } from '../models/count-list-entry.interface';

@Injectable()
export class CountContractsRestrictionsService {
  constructor(private rightsService: RightsService) {}

  public canChargeCount(): boolean {
    return this.rightsService.hasOperation(Operation.CreateCounts);
  }

  public canDeleteAtLeastOneCount(entries: ICountListEntry[]): boolean {
    return entries.some(e => this.canDeleteCount(e));
  }

  public canDeleteCount(entry: ICountListEntry): boolean {
    return this.rightsService.hasOperation(Operation.CreateCounts)
      && !!entry.count
      && entry.count.canDelete
      && entry.count.code !== CountCode.Draft;
  }

  public getAllCountEntriesSelectable(entries: ICountListEntry[]): ICountListEntry[] {
    return entries.filter(entry => !!entry.count && this.canDeleteCount(entry));
  }
}
