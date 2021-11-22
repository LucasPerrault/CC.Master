export enum OfferState {
  All = 'All',
  Archived = 'Archived',
  NoArchived = 'NoArchived',
}

export interface IOfferState {
  id: OfferState;
  name: string;
}

export const offerStates: IOfferState[] = [
  {
    id: OfferState.All,
    name: 'Tous',
  },
  {
    id: OfferState.Archived,
    name: 'Archivé',
  },
  {
    id: OfferState.NoArchived,
    name: 'Non archivé',
  },
];

export const getOfferState = (state: OfferState): IOfferState => offerStates.find(s => s.id === state);
