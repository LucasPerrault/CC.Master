export enum BillingMode {
  FlatFee = 1,
  AllUsers = 2,
  UsersWithAccess = 3,
  ActiveUsers = 4,
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
