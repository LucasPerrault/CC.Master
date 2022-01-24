import { SubdomainAvailabilityStatus } from './subdomain-availability-status.enum';

export const subdomainAvailabilityFields = 'status';
export interface ISubdomainAvailability {
  status: SubdomainAvailabilityStatus;
}
