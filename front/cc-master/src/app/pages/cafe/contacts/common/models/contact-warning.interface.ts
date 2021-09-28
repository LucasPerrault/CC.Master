export enum ContactWarningType {
  None = 'None',
  ToConfirm = 'ToConfirm',
  LeavingSoon = 'LeavingSoon',
}

export interface IContactWarning {
  type: ContactWarningType;
}
