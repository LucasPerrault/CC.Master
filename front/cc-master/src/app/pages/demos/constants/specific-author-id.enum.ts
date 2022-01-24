import { IDemoAuthor } from '../models/demo.interface';

export enum SpecificAuthorId {
  Hubspot = 0,
}

export const specificAuthors: IDemoAuthor[] = [
  {
    id: SpecificAuthorId.Hubspot,
    firstName: 'Hubspot',
    lastName: '',
  },
];

export const getSpecificAuthor = (id: SpecificAuthorId) => specificAuthors.find(a => a?.id === id);

