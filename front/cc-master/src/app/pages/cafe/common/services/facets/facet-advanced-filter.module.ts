import { NgModule } from '@angular/core';

import { FacetAdvancedFilterApiMappingService } from './facet-advanced-filter-api-mapping.service';
import { FacetAdvancedFilterConfigurationService } from './facet-advanced-filter-configuration.service';
import { FacetFormlyConfigurationService } from './facet-formly-configuration.service';

@NgModule({
  providers: [
    FacetAdvancedFilterConfigurationService,
    FacetFormlyConfigurationService,
    FacetAdvancedFilterApiMappingService,
  ],
})
export class FacetAdvancedFilterModule {}
