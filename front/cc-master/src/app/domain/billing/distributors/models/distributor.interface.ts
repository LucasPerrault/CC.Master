export const distributorFields = 'id,name,salesforceId,isDirectSales';
export interface IDistributor {
  id: number;
  salesforceId: string;
  name: string;
  isDirectSales: boolean;
}
