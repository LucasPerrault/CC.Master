import { ConfigOption } from '@ngx-formly/core';

import { FormlyFieldEnvironmentDomain } from './environment-domain/environment-domain.component';

export const luFormlyConfig: ConfigOption = {
  types: [
    {
      name: 'environment-domain',
      component: FormlyFieldEnvironmentDomain,
    },
  ],
};
