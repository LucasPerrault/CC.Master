import { IEnvironmentDomain } from '../models';

export enum EnvironmentDomain {
  ILuccaDotNet,
  EpayeCpDotCom,
  ILuccaDotCh,
  MesCongesDotNet,
  UgoOnLineDotNet,
  DauphineDotFr,
  ILuccaPreviewDotNet,
  FastobookDotCom,
  UrbaOnlineDotCom,
  Local,
  Training,
  Demo,
}

export const environmentDomains: IEnvironmentDomain[] = [
  {
    id: EnvironmentDomain.ILuccaDotNet,
    name: 'ilucca.net',
  },
  {
    id: EnvironmentDomain.EpayeCpDotCom,
    name: 'e-payecp.com',
  },
  {
    id: EnvironmentDomain.ILuccaDotCh,
    name: 'ilucca.ch',
  },
  {
    id: EnvironmentDomain.MesCongesDotNet,
    name: 'mesconges.net',
  },
  {
    id: EnvironmentDomain.UgoOnLineDotNet,
    name: 'ugo-online.net',
  },
  {
    id: EnvironmentDomain.DauphineDotFr,
    name: 'dauphine.fr',
  },
  {
    id: EnvironmentDomain.ILuccaPreviewDotNet,
    name: 'ilucca-preview.net',
  },
  {
    id: EnvironmentDomain.FastobookDotCom,
    name: 'fastobook.com',
  },
  {
    id: EnvironmentDomain.UrbaOnlineDotCom,
    name: 'urbaonline.com',
  },
  {
    id: EnvironmentDomain.Local,
    name: 'local.dev',
  },
  {
    id: EnvironmentDomain.Training,
    name: 'ilucca-test.net',
  },
  {
    id: EnvironmentDomain.Demo,
    name: 'ilucca-demo.net',
  },
];

