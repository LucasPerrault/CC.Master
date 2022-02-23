export enum FacetScope {
  Unknown = 0,
  Environment = 1,
  Establishment = 2,
}

export enum FacetType {
  Unknown = 'Unknown',
  Integer = 'Integer',
  DateTime = 'DateTime',
  Decimal = 'Decimal',
  Percentage = 'Percentage',
  // eslint-disable-next-line id-blacklist
  String = 'String',
}


export interface IFacetIdentifier {
  code: string;
  applicationId: string;
}

export interface IFacet extends IFacetIdentifier {
  id: number;
  type: FacetType;
  scope: FacetScope;
}

export const getFacetName = (facet: IFacet) => `${facet.applicationId}-${facet.code}`;

