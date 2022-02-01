export type IFormlyFieldValue = Record<string, any>;

export interface IComparisonValue {
  key: string;
  fieldValues: IFormlyFieldValue;
}
