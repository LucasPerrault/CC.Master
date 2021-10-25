import { FormlyFieldCountry } from '@cc/common/forms/formly/country/country.component';
import { FormlyFieldSpecializedContactRole } from '@cc/common/forms/formly/specialized-contact-role/specialized-contact-role.component';
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
    {
      name: 'country',
      component: FormlyFieldCountry,
    },
    {
      name: 'specialized-contact-role',
      component: FormlyFieldSpecializedContactRole,
    },
  ],
};
