import { ContractLogType } from '../constants/contract-log-type.enum';

export const contractLogFields = [
  'id',
  'name',
  'dataName',
  'dataValue',
  'idAuthor',
  'nameAuthor',
  'logDate',
  'logType',
  'logSource',
].join(',');

export interface IContractLog {
  id: number;
  name: string;
  dataName: string;
  dataValue: string;
  idAuthor: number;
  nameAuthor: string;
  logDate: Date;
  logType: ContractLogType;
  logSource: string;
}
