export interface ICountsSummaryResponse {
  all: ICountsSummary;
}

export interface ICountsSummary {
  numberOfClient: number;
  numberOfContract: number;
  sumOfAccountingNumber: number;
  sumOfNumber: number;
  sumOfTotalBillable: number;
  sumOfTotalLucca: number;
  sumOfTotalPartner: number;
}
