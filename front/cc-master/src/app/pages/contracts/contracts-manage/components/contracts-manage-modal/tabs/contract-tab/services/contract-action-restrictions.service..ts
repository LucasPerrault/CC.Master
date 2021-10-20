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

  public canEditDistributor(context: IContractValidationContext): boolean {
    return this.canEditContract()
      && !this.hasRealCounts(context)
      && !this.hasActiveEstablishments(context)
      && !this.hasContractEntries(context);
  }

  public canEditClient(context: IContractValidationContext): boolean {
    return this.canEditContract()
      && !this.hasRealCounts(context)
      && !this.hasActiveEstablishments(context)
      && !this.hasContractEntries(context);
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

  public hasContractEntries(context: IContractValidationContext): boolean {
    return this.hasLetteredContractEntries(context) && this.hasUnletteredContractEntries(context);
  }

  public hasUnletteredContractEntries(context: IContractValidationContext): boolean {
    return !!context?.contractEntries?.filter(ce => ce.letter === null).length;
  }

  public hasLetteredContractEntries(context: IContractValidationContext): boolean {
    return !!context?.contractEntries?.filter(ce => ce.letter !== null).length;
  }

  public canEditContract(): boolean {
    return this.rightsService.hasOperation(Operation.EditContracts);
  }
}
