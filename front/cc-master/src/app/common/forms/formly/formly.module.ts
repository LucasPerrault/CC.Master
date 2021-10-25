import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { EnvironmentApiSelectModule, EnvironmentDomainSelectModule } from '@cc/common/forms';
import { CountryApiSelectModule } from '@cc/common/forms/select/country-api-select/country-api-select.module';
import { FormlyModule } from '@ngx-formly/core';

import { FormlyFieldCountry } from './country/country.component';
import { FormlyFieldEnvironmentDomain } from './environment-domain/environment-domain.component';
import { FormlyFieldEnvironmentSubdomain } from './environment-subdomain/environment-subdomain.component';
import { luFormlyConfig } from './formly.config';
import { FormlyFieldSpecializedContactRole } from './specialized-contact-role/specialized-contact-role.component';
import { SpecializedContactRoleApiSelectModule } from '@cc/common/forms/select/specialized-contact-role-api-select/specialized-contact-role-api-select.module';

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
