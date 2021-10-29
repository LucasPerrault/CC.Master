import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import {
  CountryApiSelectModule,
  EnvironmentApiSelectModule,
  EnvironmentDomainSelectModule,
  SpecializedContactRoleApiSelectModule,
} from '@cc/common/forms';
import { FormlyModule } from '@ngx-formly/core';

import { FormlyFieldCountry } from './country/country.component';
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
  ],
  imports: [
    CommonModule,
    EnvironmentDomainSelectModule,
    EnvironmentApiSelectModule,
    ReactiveFormsModule,
    FormlyModule.forChild(luFormlyConfig),
    CountryApiSelectModule,
    SpecializedContactRoleApiSelectModule,
  ],
})
export class CCFormlyModule {}
