export const distributorFields = 'id,name,salesforceId,isDirectSales,isEnforcingMinimalBilling';
export interface IDistributor {
  id: number;
  salesforceId: string;
  name: string;
  isDirectSales: boolean;
  isEnforcingMinimalBilling: boolean;
}
