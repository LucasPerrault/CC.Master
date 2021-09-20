import { ICloseContractReason } from './close-contract-reason.interface';

export interface IContractClosureForm {
  closeOn: string;
  closeReason: ICloseContractReason;
}
