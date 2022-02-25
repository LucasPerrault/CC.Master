import { NgModule } from '@angular/core';

import { FacetAdvancedFilterConfigurationService } from './facet-advanced-filter-configuration.service';
import { FacetFormlyConfigurationService } from './facet-formly-configuration.service';

@NgModule({
  providers: [
    FacetAdvancedFilterConfigurationService,
    FacetFormlyConfigurationService,
  ],
})
export class FacetAdvancedFilterModule {}
