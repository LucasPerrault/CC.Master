export interface IPriceListCreationDto {
  startsOn: string;
  rows: IPriceRowCreationDto[];
}

export interface IPriceRowCreationDto {
  maxIncludedCount: number;
  unitPrice: number;
  fixedPrice: number;
}
