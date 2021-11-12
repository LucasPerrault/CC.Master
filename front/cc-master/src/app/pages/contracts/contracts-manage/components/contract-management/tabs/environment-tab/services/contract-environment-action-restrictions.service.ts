import { Injectable } from '@angular/core';
import { Operation, RightsService } from '@cc/aspects/rights';

@Injectable()
export class ContractEnvironmentActionRestrictionsService {
  constructor(private rightsService: RightsService) {}

  public canRemoveEnvironmentLinked(attachmentsNumber: number): boolean {
    return !!this.canEditContract()
      && !this.hasAttachments(attachmentsNumber);
  }

  private hasAttachments(attachmentsNumber: number): boolean {
    return !!attachmentsNumber;
  }

  private canEditContract(): boolean {
    return this.rightsService.hasOperation(Operation.EditContracts);
  }
}
