import { ConfigOption } from '@ngx-formly/core';

import { FormlyFieldApplication } from './application/application.component';
import { FormlyFieldBillingEntity } from './billing-entity/billing-entity.component';
import { FormlyFieldCluster } from './cluster/cluster.component';
import { FormlyFieldCountry } from './country/country.component';
import { FormlyFieldCriterion } from './criterion/criterion.component';
import { FormlyFieldDistributor } from './distributor/distributor.component';
import { FormlyFieldDistributorType } from './distributor-type/distributor-type.component';
import { FormlyFieldEnvironmentDomain } from './environment-domain/environment-domain.component';
import { FormlyFieldEnvironmentSubdomain } from './environment-subdomain/environment-subdomain.component';
import { FormlyFieldFacet } from './facet/facet.component';
import { FormlyFieldFacetCriterion } from './facet-criterion/facet-criterion.component';
import { FormlyFieldRange } from './range/range.component';
import { FormlyFieldSpecializedContactRole } from './specialized-contact-role/specialized-contact-role.component';
import { FormlyFieldFacetValue } from './facet-value/facet-value.component';

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
    {
      name: 'application',
      component: FormlyFieldApplication,
    },
    {
      name: 'distributor',
      component: FormlyFieldDistributor,
    },
    {
      name: 'distributorType',
      component: FormlyFieldDistributorType,
    },
    {
      name: 'billing-entity',
      component: FormlyFieldBillingEntity,
    },
    {
      name: 'criterion',
      component: FormlyFieldCriterion,
    },
    {
      name: 'facet',
      component: FormlyFieldFacet,
    },
    {
      name: 'facet-criterion',
      component: FormlyFieldFacetCriterion,
    },
    {
      name: 'facet-value',
      component: FormlyFieldFacetValue,
    },
    {
      name: 'range',
      component: FormlyFieldRange,
    },
  ],
};
