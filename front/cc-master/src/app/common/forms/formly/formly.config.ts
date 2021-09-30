import { ConfigOption } from '@ngx-formly/core';

import { FormlyFieldEnvironmentDomain } from './environment-domain/environment-domain.component';
import { FormlyFieldEnvironmentSubdomain } from './environment-subdomain/environment-subdomain.component';

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
  ],
};
