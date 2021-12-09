export enum BillingMode {
  FlatFee = 'FlatFee',
  AllUsers = 'AllUsers',
  UsersWithAccess = 'UsersWithAccess',
  ActiveUsers = 'ActiveUsers',
}

export interface IBillingMode {
  id: BillingMode;
  name: string;
}

export const billingModes: IBillingMode[] = [
  {
    id: BillingMode.FlatFee,
    name: 'offers_billingMode_flatFee',
  },
  {
    id: BillingMode.AllUsers,
    name: 'offers_billingMode_allUsers',
  },
  {
    id: BillingMode.UsersWithAccess,
    name: 'offers_billingMode_usersWithAccess',
  },
  {
    id: BillingMode.ActiveUsers,
    name: 'offers_billingMode_activeUsers',
  },
];

export const getBillingMode = (id: BillingMode): IBillingMode => billingModes.find(b => b.id === id);
