import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { BillingEntitySelectModule, EnvironmentDomainSelectModule } from '@cc/common/forms';
import { RangeModule } from '@cc/common/forms/input/range/range.module';
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
import { ComparisonCriterionSelectModule } from '../select/comparison-criterion-select/comparison-criterion-select.module';
import { FacetApiSelectModule } from '../select/facet-api-select/facet-api-select.module';
import { FacetComparisonCriterionSelectModule } from '../select/facet-comparison-criterion-select/facet-comparison-criterion-select.module';
import { FacetValueApiSelectModule } from '../select/facet-value-api-select/facet-value-api-select.module';
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
import { FormlyFieldFacetValue } from './facet-value/facet-value.component';
import { luFormlyConfig } from './formly.config';
import { FormlyFieldRange } from './range/range.component';
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
    FormlyFieldBillingEntity,
    FormlyFieldCriterion,
    FormlyFieldFacet,
    FormlyFieldFacetCriterion,
    FormlyFieldRange,
    FormlyFieldFacetValue,
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
    BillingEntitySelectModule,
    LuSelectInputModule,
    LuOptionPickerModule,
    LuOptionFeederModule,
    LuOptionSearcherModule,
    LuOptionItemModule,
    LuInputClearerModule,
    LuForOptionsModule,
    LuInputDisplayerModule,
    ComparisonCriterionSelectModule,
    FacetApiSelectModule,
    FacetComparisonCriterionSelectModule,
    RangeModule,
    FacetValueApiSelectModule,
  ],
})
export class CCFormlyModule {}
