export class InstancesApiRoute {
  public static base = '/api/v3/instances';
  public static id = (id: number) => `${ InstancesApiRoute.base }/${ id }`;
  public static users = (id: number) => `${ InstancesApiRoute.base }/${ id }/users`;
  public static lock = (id: number) => `${ InstancesApiRoute.id(id) }/lock`;
  public static unlock = (id: number) => `${ InstancesApiRoute.id(id) }/unlock`;
  public static connectAs = (id: number) => `${ InstancesApiRoute.id(id) }/connectAs`;
  public static editPassword = (id: number, password: string) => `${ InstancesApiRoute.id(id) }/ChangePassword?password=${ password }`;
}
