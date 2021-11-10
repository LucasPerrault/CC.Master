import { TranslateModule } from '@cc/aspects/translate';
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
import { FormlyFieldDistributorType } from './distributor-type/distributor-type.component';
import { FormlyFieldEnvironmentDomain } from './environment-domain/environment-domain.component';
import { FormlyFieldEnvironmentSubdomain } from './environment-subdomain/environment-subdomain.component';
import { luFormlyConfig } from './formly.config';
import { FormlyFieldSpecializedContactRole } from './specialized-contact-role/specialized-contact-role.component';
import { LuSelectInputModule } from '@lucca-front/ng/select';
import { LuInputClearerModule, LuInputDisplayerModule } from '@lucca-front/ng/input';
import { LuOptionPickerModule, LuOptionFeederModule, LuOptionSearcherModule, LuOptionItemModule, LuForOptionsModule } from '@lucca-front/ng/option';

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
