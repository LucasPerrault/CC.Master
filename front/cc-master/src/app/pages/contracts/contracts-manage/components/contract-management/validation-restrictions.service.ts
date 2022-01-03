import { Inject, Injectable } from '@angular/core';
import { DistributorIds, IPrincipal, PRINCIPAL } from '@cc/aspects/principal';
import { Operation, RightsService } from '@cc/aspects/rights';

import { IValidationContext } from './validation-context-store.data';

@Injectable()
export class ValidationRestrictionsService {

  constructor(
    @Inject(PRINCIPAL) private principal: IPrincipal,
    private rightsService: RightsService,
  ) {}

  public canDeleteContracts(context: IValidationContext): boolean {
    return DistributorIds.isLuccaUser(this.principal)
      && this.canEditContract(context)
      && !this.hasRealCounts(context)
      && !this.hasActiveEstablishments(context)
      && !this.hasUnletteredContractEntries(context);
  }

  public hasRealCounts(context: IValidationContext): boolean {
    return !!context?.realCounts?.length;
  }

  public hasActiveEstablishments(context: IValidationContext): boolean {
    return !!context?.activeEstablishmentNumber;
  }

  public hasUnletteredContractEntries(context: IValidationContext): boolean {
    return !!context?.contractEntries?.filter(ce => ce.letter === null).length;
  }

  public canEditContract(context: IValidationContext): boolean {
    return this.hasRightsToEditContracts
      && this.hasRightsToReadValidationContext
      && !!context;
  }

  public get hasRightsToEditContracts(): boolean {
    return this.rightsService.hasOperation(Operation.EditContracts);
  }

  public get hasRightsToReadValidationContext(): boolean {
    return this.canReadCount && this.canReadContractEntries;
  }

  public get canReadCount(): boolean {
    return this.rightsService.hasOperation(Operation.ReadCounts);
  }

  public get canReadContractEntries(): boolean {
    return this.rightsService.hasOperation(Operation.ReadContractEntries);
  }
}
