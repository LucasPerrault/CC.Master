export interface IContractEntry {
  id: number;
  letter: number;
}

export interface IContractValidationContext {
  activeEstablishmentsNumber: number;
  realCountNumber: number;
  contractEntries: IContractEntry[];
}
