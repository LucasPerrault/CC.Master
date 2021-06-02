import { INavigationTab } from '@cc/common/navigation';

import { NoNavPath } from './no-nav-path.enum';

export const noNavTabs: INavigationTab[] = [
  {
    name: 'front_errorPages_authorizeIp_title',
    url: NoNavPath.AuthorizeIp,
  },
  {
    name: 'front_errorPages_invalidIp_title',
    url: NoNavPath.InvalidIp,
  },
  {
    name: 'front_errorPages_ipAuthorizationRequest_title',
    url: NoNavPath.IpAuthorizationRequest,
  },
  {
    name: 'front_errorPages_wrongEmailDomain_title',
    url: NoNavPath.WrongEmailDomain,
  },
  {
    name: 'front_errorPages_forbidden_title',
    url: NoNavPath.Forbidden,
  },
  {
    name: 'front_errorPages_notFound_title',
    url: NoNavPath.NotFound,
  },
];
