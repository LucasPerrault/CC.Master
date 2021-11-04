import { CloseContractReason } from '../constants/close-contract-reason.enum';

export interface IContractClosureDto {
  closeOn: string;
  closeReason: CloseContractReason;
}
