export const priceRowFields = 'id,maxIncludedCount,unitPrice,fixedPrice,listId';

export interface IPriceRow {
  id: number;
  maxIncludedCount: number;
  unitPrice: number;
  fixedPrice: number;
  listId: number;
}
