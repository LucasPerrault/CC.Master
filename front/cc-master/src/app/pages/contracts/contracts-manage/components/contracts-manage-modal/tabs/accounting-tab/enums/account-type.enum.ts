export enum AccountType {
  Lucca = 401,
  Client = 411,
}

export interface IAccount {
  type: AccountType;
  name: string;
}

export const accounts: IAccount[] = [
  {
    type: AccountType.Lucca,
    name: 'Provision',
  },
  {
    type: AccountType.Client,
    name: 'Client',
  },
];

export const getAccount = (accountType: AccountType) =>
  accounts.find(account => account.type === accountType);

