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
    name: 'offers_filters_state_all',
  },
  {
    id: OfferState.Archived,
    name: 'offers_filters_state_archived',
  },
  {
    id: OfferState.NoArchived,
    name: 'offers_filters_state_noarchived',
  },
];

export const getOfferState = (state: OfferState): IOfferState => offerStates.find(s => s.id === state);
