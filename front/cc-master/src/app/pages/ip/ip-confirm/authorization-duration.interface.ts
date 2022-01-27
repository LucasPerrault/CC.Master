export enum AuthorizationDuration {
  OneDay = 0,
  SixMonths = 1,
}

export interface IAuthorizationDuration {
  id: AuthorizationDuration;
  label: string;
  description: string;
}

export const authorizationDurations: IAuthorizationDuration[] = [
  {
    id: AuthorizationDuration.OneDay,
    label: 'ip_authorization_one_day',
    description: 'ip_authorization_one_day_recommendation',
  },
  {
    id: AuthorizationDuration.SixMonths,
    label: 'ip_authorization_six_months',
    description: 'ip_authorization_six_months_recommendation',
  },
];

export const getAuthorization = (id: AuthorizationDuration) => authorizationDurations.find(a => a.id === id);

