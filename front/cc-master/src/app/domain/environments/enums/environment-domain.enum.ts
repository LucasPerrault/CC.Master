import { IEnvironmentDomain } from '@cc/domain/environments';

export enum EnvironmentDomain {
  ILuccaDotNet = 0,
  ILuccaDotCh = 2,
  DauphineDotFr = 5,
}

export const environmentDomains: IEnvironmentDomain[] = [
  {
    id: EnvironmentDomain.ILuccaDotNet,
    name: 'ilucca.net',
  },
  {
    id: EnvironmentDomain.ILuccaDotCh,
    name: 'ilucca.ch',
  },
  {
    id: EnvironmentDomain.DauphineDotFr,
    name: 'dauphine.fr',
  },
];

