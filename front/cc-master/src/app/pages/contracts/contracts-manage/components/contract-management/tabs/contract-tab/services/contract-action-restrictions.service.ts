import { Injectable } from '@angular/core';
import { RightsService } from '@cc/aspects/rights';

import { IValidationContext } from '../../../validation-context-store.data';
import { ValidationRestrictionsService } from '../../../validation-restrictions.service';

@Injectable()
export class ContractActionRestrictionsService {

  constructor(private rightsService: RightsService, private commonRestrictionsService: ValidationRestrictionsService) {}

  public canEditContract(context: IValidationContext): boolean {
    return this.commonRestrictionsService.canEditContract(context);
  }

  public canEditDistributor(context: IValidationContext): boolean {
    return this.canEditContract(context)
      && !this.commonRestrictionsService.hasRealCounts(context)
      && !this.commonRestrictionsService.hasActiveEstablishments(context)
      && !this.hasContractEntries(context);
  }

  public canEditClient(context: IValidationContext): boolean {
    return this.canEditContract(context)
      && !this.commonRestrictionsService.hasRealCounts(context)
      && !this.commonRestrictionsService.hasActiveEstablishments(context)
      && !this.hasContractEntries(context);
  }

  public canEditTheoreticalStartOn(context: IValidationContext): boolean {
    return this.canEditContract(context)
      && !this.commonRestrictionsService.hasRealCounts(context)
      && !this.commonRestrictionsService.hasActiveEstablishments(context);
  }

  public canEditOffer(context: IValidationContext): boolean {
    return this.canEditContract(context)
      && !this.commonRestrictionsService.hasRealCounts(context)
      && !this.commonRestrictionsService.hasUnletteredContractEntries(context);
  }

  public canEditProduct(context: IValidationContext): boolean {
    return this.canEditContract(context)
      && !this.commonRestrictionsService.hasRealCounts(context)
      && !this.commonRestrictionsService.hasUnletteredContractEntries(context)
      && !this.commonRestrictionsService.hasActiveEstablishments(context);
  }

  public canEditMinimalBilling(context: IValidationContext): boolean {
    return this.canEditContract(context)
      && !this.commonRestrictionsService.hasRealCounts(context);
  }

  public canEditBillingFrequency(context: IValidationContext): boolean {
    return this.canEditContract(context)
      && !this.commonRestrictionsService.hasRealCounts(context)
      && !this.commonRestrictionsService.hasActiveEstablishments(context);
  }

  public canEditWithSimilarOffer(): boolean {
    return this.commonRestrictionsService.hasRightsToEditContracts && this.commonRestrictionsService.canReadCount;
  }

  private hasContractEntries(context: IValidationContext): boolean {
    return this.canEditContract(context) && !!context?.contractEntries?.length;
  }
}
