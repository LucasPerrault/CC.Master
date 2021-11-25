import { Injectable } from '@angular/core';
import { ICount } from '@cc/domain/billing/counts';

import { EstablishmentType } from '../constants/establishment-type.enum';
import { IEstablishmentActionsContext } from '../models/establishment-actions-context.interface';
import { IEstablishmentContract } from '../models/establishment-contract.interface';

@Injectable()
export class EstablishmentActionContextService {
  public getActionContext(
    contract: IEstablishmentContract,
    realCounts: ICount[],
    establishmentType: EstablishmentType,
  ): IEstablishmentActionsContext {
    const lastCountPeriod = this.getLastCountPeriod(realCounts);
    return { contract, realCounts, lastCountPeriod, establishmentType };
  }

  private getLastCountPeriod(realCounts: ICount[]): Date | null {
    if (!realCounts?.length) {
      return null;
    }

    const sortedDescCounts = realCounts.sort((a, b) =>
      new Date(b.countPeriod).getTime() - new Date(a.countPeriod).getTime());

    const lastCountPeriod = sortedDescCounts[0]?.countPeriod;
    return !!lastCountPeriod ? new Date(lastCountPeriod) : null;
  }
}
