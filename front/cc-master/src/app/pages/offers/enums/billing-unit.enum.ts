export enum BillingUnit {
  None = 'None',
  Users = 'Users',
  ActiveUsers = 'ActiveUsers',
  Declarers = 'Declarers',
  Cards = 'Cards',
  DownloadedDocuments = 'DownloadedDocuments',
  SynchronizedAccounts = 'SynchronizedAccounts',
  Licenses = 'Licenses',
  Servers = 'Servers',
}

export interface IBillingUnit {
  id: BillingUnit;
  name: string;
}

export const billingUnits: IBillingUnit[] = [
  {
    id: BillingUnit.None,
    name: 'offers_billingUnit_no',
  },
  {
    id: BillingUnit.Users,
    name: 'offers_billingUnit_users',
  },
  {
    id: BillingUnit.ActiveUsers,
    name: 'offers_billingUnit_activeUsers',
  },
  {
    id: BillingUnit.Declarers,
    name: 'offers_billingUnit_declarers',
  },
  {
    id: BillingUnit.Cards,
    name: 'offers_billingUnit_cards',
  },
  {
    id: BillingUnit.DownloadedDocuments,
    name: 'offers_billingUnit_downloadedDocuments',
  },
  {
    id: BillingUnit.SynchronizedAccounts,
    name: 'offers_billingUnit_synchronizedAccounts',
  },
  {
    id: BillingUnit.Licenses,
    name: 'offers_billingUnit_licenses',
  },
  {
    id: BillingUnit.Servers,
    name: 'offers_billingUnit_servers',
  },
];

export const getBillingUnit = (id: BillingUnit): IBillingUnit => billingUnits.find(b => b.id === id);
