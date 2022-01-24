// this enum must be identical to the back project enum in SubdomainResponse.cs
export enum SubdomainAvailabilityStatus {
  Ok = 0,
  EmptyString,
  SubdomainReservedWord,
  SubdomainStartWithReservedWord,
  SubdomainAlreadyTakenByEnvironment,
  SubdomainAlreadyTakenByDemo,
  SubdomainAlreadyTakenByProtectedDemo,
  ContainsInvalidChars,
  SubdomainTooShort,
  SubdomainTooLong,
}

interface ISubdomainAvailabilityStatus {
  id: SubdomainAvailabilityStatus;
  message: string;
}

export const subdomainAvailabilityStatus: ISubdomainAvailabilityStatus[] = [
  {
    id: SubdomainAvailabilityStatus.Ok,
    message: 'demos_subdomain_availability_status_ok',
  },
  {
    id: SubdomainAvailabilityStatus.EmptyString,
    message: 'demos_subdomain_availability_status_empty',
  },
  {
    id: SubdomainAvailabilityStatus.SubdomainReservedWord,
    message: 'demos_subdomain_availability_status_reserved',
  },
  {
    id: SubdomainAvailabilityStatus.SubdomainStartWithReservedWord,
    message: 'demos_subdomain_availability_status_start_with_reserved_word',
  },
  {
    id: SubdomainAvailabilityStatus.SubdomainAlreadyTakenByEnvironment,
    message: 'demos_subdomain_availability_status_taken_by_environment',
  },
  {
    id: SubdomainAvailabilityStatus.SubdomainAlreadyTakenByDemo,
    message: 'demos_subdomain_availability_status_taken_by_demo',
  },
  {
    id: SubdomainAvailabilityStatus.SubdomainAlreadyTakenByProtectedDemo,
    message: 'demos_subdomain_availability_status_taken_by_protected_demo',
  },
  {
    id: SubdomainAvailabilityStatus.ContainsInvalidChars,
    message: 'demos_subdomain_availability_status_invalid',
  },
  {
    id: SubdomainAvailabilityStatus.SubdomainTooShort,
    message: 'demos_subdomain_availability_status_too_short',
  },
  {
    id: SubdomainAvailabilityStatus.SubdomainTooLong,
    message: 'demos_subdomain_availability_status_too_long',
  },
];

export const getSubdomainAvailabilityStatus = (status: SubdomainAvailabilityStatus): ISubdomainAvailabilityStatus =>
  subdomainAvailabilityStatus.find(s => s.id === status);
