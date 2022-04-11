import { Component, Input } from '@angular/core';

import { FacetType, IAdditionalColumn, IFacet } from '../../../common/models';
import { IEstablishment } from '../../../common/models/establishment.interface';
import { IEstablishmentFacetValue } from '../../../common/models/facet-value.interface';
import { FacetPipeOptions } from '../../../common/pipes/facet.pipe';
import { EstablishmentAdditionalColumn } from '../../models/establishment-additional-column';

@Component({
  selector: 'cc-establishment-list',
  templateUrl: './establishment-list.component.html',
  styleUrls: ['./establishment-list.component.scss'],
})
export class EstablishmentListComponent {
  @Input() public establishments: IEstablishment[];
  @Input() public selectedColumns: IAdditionalColumn[];
  @Input() public facets: IFacet[];

  public additionalColumn = EstablishmentAdditionalColumn;

  public trackBy(index: number, establishment: IEstablishment): number {
    return establishment.id;
  }

  public isHidden(column: EstablishmentAdditionalColumn): boolean {
    return !this.selectedColumns.find(c => c.id === column);
  }

  public getEnvironmentName(subdomain: string, domain: string): string {
    return `${ subdomain }.${ domain }`;
  }

  public getFacetValue(facetValues: IEstablishmentFacetValue[], facet: IFacet) {
    return facetValues?.filter(v => v.facetId === facet.id);
  }

  public getFacetOptions(facet: IFacet): FacetPipeOptions {
    switch (facet.type) {
      case FacetType.DateTime:
        return { format: 'shortDate' };
    }
  }
}
