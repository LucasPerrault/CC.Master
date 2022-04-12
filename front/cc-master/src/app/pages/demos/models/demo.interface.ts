export interface IDemoInstance {
  id: number;
  isProtected: boolean;
  allUsersImposedPassword: string;
}

export interface IDemoAuthor {
  id: number;
  firstName: string;
  lastName: string;
}

export interface ITemplateDemo {
  id: number;
  subdomain: string;
}

export interface IDemo {
  id: number;
  subdomain: string;
  createdAt: Date;
  deletionScheduledOn: Date;
  isActive: boolean;
  authorId: number;
  author: IDemoAuthor;
  instanceID: number;
  instance: IDemoInstance;
  comment: string;

  href: string;
}

export const demoDomain = '.ilucca-demo.net';
