import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import {
  ClusterApiSelectModule,
  CountryApiSelectModule,
  EnvironmentDomainSelectModule,
  SpecializedContactRoleApiSelectModule,
} from '@cc/common/forms';
import { FormlyModule } from '@ngx-formly/core';

import { ApplicationApiSelectModule, DistributorApiSelectModule,EnvironmentApiSelectModule } from '../../../pages/cafe/common/forms';
import { FormlyFieldApplication } from './application/application.component';
import { FormlyFieldCluster } from './cluster/cluster.component';
import { FormlyFieldCountry } from './country/country.component';
import { FormlyFieldDistributor } from './distributor/distributor.component';
import { FormlyFieldEnvironmentDomain } from './environment-domain/environment-domain.component';
import { FormlyFieldEnvironmentSubdomain } from './environment-subdomain/environment-subdomain.component';
import { luFormlyConfig } from './formly.config';
import { FormlyFieldSpecializedContactRole } from './specialized-contact-role/specialized-contact-role.component';

@NgModule({
  declarations: [
    FormlyFieldEnvironmentDomain,
    FormlyFieldEnvironmentSubdomain,
    FormlyFieldCountry,
    FormlyFieldSpecializedContactRole,
    FormlyFieldCluster,
    FormlyFieldApplication,
    FormlyFieldDistributor,
  ],
  imports: [
    CommonModule,
    EnvironmentDomainSelectModule,
    EnvironmentApiSelectModule,
    ApplicationApiSelectModule,
    DistributorApiSelectModule,
    ReactiveFormsModule,
    FormlyModule.forChild(luFormlyConfig),
    CountryApiSelectModule,
    SpecializedContactRoleApiSelectModule,
    ClusterApiSelectModule,
  ],
})
export class CCFormlyModule {}
