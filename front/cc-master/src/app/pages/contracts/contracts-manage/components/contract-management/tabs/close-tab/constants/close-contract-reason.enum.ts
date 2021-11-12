import { ICloseContractReason } from '../models/close-contract-reason.interface';

export enum CloseContractReason {
  Modification = 1,
  Resiliation = 2,
}

export const closeContractReasons: ICloseContractReason[] = [
  {
    id: CloseContractReason.Resiliation,
    name: 'front_contractPage_endContractReason_resiliation',
  },
  {
    id: CloseContractReason.Modification,
    name: 'front_contractPage_endContractReason_modification',
  },
];

export const getCloseContractReason = (id: CloseContractReason): ICloseContractReason =>
  closeContractReasons.find(r => r.id === id);
