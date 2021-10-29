import { ConfigOption } from '@ngx-formly/core';

import { FormlyFieldCluster } from './cluster/cluster.component';
import { FormlyFieldCountry } from './country/country.component';
import { FormlyFieldEnvironmentDomain } from './environment-domain/environment-domain.component';
import { FormlyFieldEnvironmentSubdomain } from './environment-subdomain/environment-subdomain.component';
import { FormlyFieldSpecializedContactRole } from './specialized-contact-role/specialized-contact-role.component';

export const luFormlyConfig: ConfigOption = {
  types: [
    {
      name: 'environment-domain',
      component: FormlyFieldEnvironmentDomain,
    },
    {
      name: 'environment-subdomain',
      component: FormlyFieldEnvironmentSubdomain,
    },
    {
      name: 'country',
      component: FormlyFieldCountry,
    },
    {
      name: 'specialized-contact-role',
      component: FormlyFieldSpecializedContactRole,
    },
    {
      name: 'cluster',
      component: FormlyFieldCluster,
    },
  ],
};
