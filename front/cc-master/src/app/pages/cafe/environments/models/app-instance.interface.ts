export interface IAppInstance {
  id: number;
  name: string;
  applicationId: string;
  environmentId: number;
  deletedAt: string;
}

export interface IApplication {
  id: string;
  name: string;
}
