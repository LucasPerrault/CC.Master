export class DistributorUtilsService {
  public static getOnlyName(codeAndName: string): string {
    const codeAndNameSeparator = '-';
    const separatorIndex = codeAndName.indexOf(codeAndNameSeparator);
    return codeAndName.slice(separatorIndex + 1);
  }
}
