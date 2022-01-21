export enum BillingEntity {
  France = 1,
  Iberia = 2,
}

export interface IBillingEntity {
  id: BillingEntity;
  name: string;
}

export const billingEntities: IBillingEntity[] = [
  {
    id: BillingEntity.France,
    name: 'billingEntity_france',
  },
  {
    id: BillingEntity.Iberia,
    name: 'billingEntity_iberia',
  },
];

export const getBillingEntity = (id: BillingEntity): IBillingEntity =>
  billingEntities.find(billingEntity => billingEntity.id === id);

