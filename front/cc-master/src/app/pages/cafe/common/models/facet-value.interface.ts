import { FacetType, IFacetIdentifier } from './facet.interface';

export interface IFacetValue {
  facetId: number;
  environmentId: number;
  facetIdentifier: IFacetIdentifier;
}

export type FacetValue = IEnvironmentFacetValue | IEstablishmentFacetValue;

export interface IEnvironmentFacetValue extends IFacetValue {
  id: number;
  type: FacetType;
  facetId: number;
  environmentId: number;
  facetIdentifier: IFacetIdentifier;

  value: any;
}

export interface IEstablishmentFacetValue extends IFacetValue {
  id: number;
  type: FacetType;
  facetId: number;
  environmentId: number;
  establishmentId: number;
  facetIdentifier: IFacetIdentifier;

  value: any;
}
