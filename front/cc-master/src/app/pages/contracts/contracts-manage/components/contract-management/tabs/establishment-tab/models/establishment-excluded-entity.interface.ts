import { ISolution } from '@cc/domain/billing/offers';

export const excludedEntitiesFields = 'id,solutionId,solution[id,name],legalEntityID';

export interface IEstablishmentExcludedEntity {
  id: number;
  solutionId: number;
  solution: ISolution;
  legalEntityID: number;
}
