import { Observable, pipe, UnaryFunction } from 'rxjs';
import { map } from 'rxjs/operators';

import { getFacetName, IAdditionalColumn, IFacet } from '../../../models';

export interface IFacetAndColumn {
  id: number | string;
  name: string;
  facet?: IFacet;
  column?: IAdditionalColumn;
}

export class FacetAndColumnHelper {
  public static get toFacets(): UnaryFunction<Observable<IFacetAndColumn[]>, Observable<IFacet[]>> {
    return pipe(map(fAndC => FacetAndColumnHelper.getFacets(fAndC)));
  }

  public static getFacets(fAndC: IFacetAndColumn[]): IFacet[] {
    return fAndC?.filter(f => !!f?.facet)?.map(f => f.facet) ?? [];
  }

  public static getColumns(fAndC: IFacetAndColumn[]): IAdditionalColumn[] {
    return fAndC?.filter(f => !!f?.column)?.map(f => f.column) ?? [];
  }

  public static transformColumnToFacetAndColumn(column: IAdditionalColumn): IFacetAndColumn {
    return { id: column.id, name: column.name, column };
  }

  public static transformFacetToFacetAndColumn(facet: IFacet): IFacetAndColumn {
    return { id: facet.id, name: getFacetName(facet), facet };
  }

  public static get mapFacetsToFacetAndColumns(): UnaryFunction<Observable<IFacet[]>, Observable<IFacetAndColumn[]>> {
    return pipe(map(facets => facets.map(facet => FacetAndColumnHelper.transformFacetToFacetAndColumn(facet))));
  }

  public static mapColumnsToFacetAndColumns(columns: IAdditionalColumn[]): IFacetAndColumn[] {
    return columns.map(column => FacetAndColumnHelper.transformColumnToFacetAndColumn(column));
  }
}
