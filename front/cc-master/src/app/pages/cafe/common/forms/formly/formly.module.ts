import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { EnvironmentDomainSelectModule } from '@cc/common/forms';
import { LuInputClearerModule, LuInputDisplayerModule } from '@lucca-front/ng/input';
import {
  LuForOptionsModule,
  LuOptionFeederModule,
  LuOptionItemModule,
  LuOptionPickerModule,
  LuOptionSearcherModule,
} from '@lucca-front/ng/option';
import { LuSelectInputModule } from '@lucca-front/ng/select';
import { FormlyModule } from '@ngx-formly/core';

import {
  ApplicationApiSelectModule,
  ClusterApiSelectModule,
  CountryApiSelectModule,
  DistributorApiSelectModule,
  EnvironmentApiSelectModule,
  SpecializedContactRoleApiSelectModule,
} from '../index';
import { FormlyFieldApplication } from './application/application.component';
import { FormlyFieldCluster } from './cluster/cluster.component';
import { FormlyFieldCountry } from './country/country.component';
import { FormlyFieldDistributor } from './distributor/distributor.component';
import { FormlyFieldDistributorType } from './distributor-type/distributor-type.component';
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
    FormlyFieldDistributorType,
  ],
  imports: [
    CommonModule,
    EnvironmentDomainSelectModule,
    EnvironmentApiSelectModule,
    ApplicationApiSelectModule,
    DistributorApiSelectModule,
    ReactiveFormsModule,
    TranslateModule,
    FormlyModule.forChild(luFormlyConfig),
    CountryApiSelectModule,
    SpecializedContactRoleApiSelectModule,
    ClusterApiSelectModule,
    LuSelectInputModule,
    LuOptionPickerModule,
    LuOptionFeederModule,
    LuOptionSearcherModule,
    LuOptionItemModule,
    LuInputClearerModule,
    LuForOptionsModule,
    LuInputDisplayerModule,
  ],
})
export class CCFormlyModule {}
