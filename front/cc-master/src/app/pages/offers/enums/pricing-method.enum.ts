export enum PricingMethod {
  Constant = 'Constant',
  Linear = 'Linear',
  AnnualCommitment = 'AnnualCommitment',
}

export const pricingMethods = [
  PricingMethod.AnnualCommitment,
  PricingMethod.Constant,
  PricingMethod.Linear,
];
