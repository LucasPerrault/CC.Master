import { Injectable } from '@angular/core';

import { IValidationContext } from '../../../validation-context-store.data';
import { ValidationRestrictionsService } from '../../../validation-restrictions.service';


@Injectable()
export class ContractActionRestrictionsService {
  constructor(private restrictionsService: ValidationRestrictionsService) {}

  public canEditDistributor(context: IValidationContext): boolean {
    return this.canEditContract()
      && !this.hasRealCounts(context)
      && !this.hasActiveEstablishments(context)
      && !this.hasContractEntries(context);
  }

  public canEditClient(context: IValidationContext): boolean {
    return this.canEditContract()
      && !this.hasRealCounts(context)
      && !this.hasActiveEstablishments(context)
      && !this.hasContractEntries(context);
  }

  public canEditTheoreticalStartOn(context: IValidationContext): boolean {
    return this.canEditContract()
      && !this.hasRealCounts(context)
      && !this.hasActiveEstablishments(context);
  }

  public canEditOffer(context: IValidationContext): boolean {
    return this.canEditContract()
      && !this.hasRealCounts(context)
      && !this.hasUnletteredContractEntries(context);
  }

  public canEditProduct(context: IValidationContext): boolean {
    return this.canEditContract()
      && !this.hasRealCounts(context)
      && !this.hasUnletteredContractEntries(context)
      && !this.hasActiveEstablishments(context);
  }

  public canEditMinimalBilling(context: IValidationContext): boolean {
    return this.canEditContract()
      && !this.hasRealCounts(context);
  }

  public canEditBillingFrequency(context: IValidationContext): boolean {
    return this.canEditContract()
      && !this.hasRealCounts(context)
      && !this.hasActiveEstablishments(context);
  }

  public hasActiveEstablishments(context: IValidationContext): boolean {
    return this.restrictionsService.hasActiveEstablishments(context);
  }

  public hasRealCounts(context: IValidationContext): boolean {
    return this.restrictionsService.hasRealCounts(context);
  }

  public hasContractEntries(context: IValidationContext): boolean {
    return !!context?.contractEntries?.length;
  }

  public hasUnletteredContractEntries(context: IValidationContext): boolean {
    return this.restrictionsService.hasUnletteredContractEntries(context);
  }

  public hasLetteredContractEntries(context: IValidationContext): boolean {
    return !!context?.contractEntries?.filter(ce => ce.letter !== null).length;
  }

  public canEditContract(): boolean {
    return this.restrictionsService.canEditContract();
  }
}
