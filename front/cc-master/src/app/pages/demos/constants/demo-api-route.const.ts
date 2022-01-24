export class DemosApiRoute {
  public static base = '/api/demos';
  public static duplicate = `${ DemosApiRoute.base }/duplicate`;
  public static id = (id: number) => `${ DemosApiRoute.base }/${ id }`;
}
