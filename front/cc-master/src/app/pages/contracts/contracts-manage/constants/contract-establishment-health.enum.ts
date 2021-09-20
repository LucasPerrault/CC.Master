import { IContractEstablishmentHealth } from '../models/contract-establishment-health.interface';

export enum ContractEstablishmentHealth {
  Ok = 0,
  Error = 1,
  NoEnvironment = 2,
  NoEstablishment = 3,
}

export const contractEstablishmentsHealth: IContractEstablishmentHealth[] = [
  {
    id: ContractEstablishmentHealth.Ok,
    name: 'front_contractPage_establishmentHealth_ok',
    icon: '',
  },
  {
    id: ContractEstablishmentHealth.Error,
    name: 'front_contractPage_establishmentHealth_error',
    icon: 'icon-error u-textError',
  },
  {
    id: ContractEstablishmentHealth.NoEnvironment,
    name: 'front_contractPage_establishmentHealth_noEnvironment',
    icon: 'icon-warning u-textWarning',
  },
  {
    id: ContractEstablishmentHealth.NoEstablishment,
    name: 'front_contractPage_establishmentHealth_noEstablishment',
    icon: 'icon-warning u-textWarning',
  },
];
