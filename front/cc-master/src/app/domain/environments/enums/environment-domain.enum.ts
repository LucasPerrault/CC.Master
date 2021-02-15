import { IEnvironmentDomain } from '@cc/domain/environments';

export enum EnvironmentDomain {
  ILuccaDotNet = 0,
  EpayeCpDotCom = 1,
  ILuccaDotCh = 2,
  MesCongesDotNet = 3,
  UgoOnLineDotNet = 4,
  DauphineDotFr = 5,
  ILuccaPreviewDotNet = 6,
  FastobookDotCom = 7,
  UrbaOnlineDotCom = 8,
  Local = 9,
  Training = 10,
  Demo = 11,
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

