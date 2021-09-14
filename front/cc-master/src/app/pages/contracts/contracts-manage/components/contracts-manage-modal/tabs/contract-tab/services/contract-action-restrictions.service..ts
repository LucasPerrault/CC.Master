import { Injectable } from '@angular/core';
import { Operation, RightsService } from '@cc/aspects/rights';

import { IContractValidationContext } from '../models/contract-validation-context.interface';

@Injectable()
export class ContractActionRestrictionsService {
  constructor(private rightsService: RightsService) {}

  public canDeleteContracts(context: IContractValidationContext): boolean {
    return this.canEditContract()
      && !this.hasRealCounts(context)
      && !this.hasActiveEstablishments(context)
      && !this.hasUnletteredContractEntries(context);
  }

  public canEditTheoreticalStartOn(context: IContractValidationContext): boolean {
    return this.canEditContract()
      && !this.hasRealCounts(context)
      && !this.hasActiveEstablishments(context);
  }

  public canEditOffer(context: IContractValidationContext): boolean {
    return this.canEditContract()
      && !this.hasRealCounts(context)
      && !this.hasUnletteredContractEntries(context);
  }

  public canEditProduct(context: IContractValidationContext): boolean {
    return this.canEditContract()
      && !this.hasRealCounts(context)
      && !this.hasUnletteredContractEntries(context)
      && !this.hasActiveEstablishments(context);
  }

  public canEditMinimalBilling(context: IContractValidationContext): boolean {
    return this.canEditContract()
      && !this.hasRealCounts(context);
  }

  public canEditBillingFrequency(context: IContractValidationContext): boolean {
    return this.canEditContract()
      && !this.hasRealCounts(context)
      && !this.hasActiveEstablishments(context);
  }

  public hasActiveEstablishments(context: IContractValidationContext): boolean {
    return !!context?.activeEstablishmentsNumber;
  }

  public hasRealCounts(context: IContractValidationContext): boolean {
    return !!context?.realCountNumber;
  }

  public hasUnletteredContractEntries(context: IContractValidationContext): boolean {
    return !!context?.unletteredContractEntriesNumber;
  }

  public canEditContract(): boolean {
    return this.rightsService.hasOperation(Operation.EditContracts);
  }
}
