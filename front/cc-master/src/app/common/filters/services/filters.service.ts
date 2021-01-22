import { Injectable } from '@angular/core';
import { IFilterParams } from '@cc/common/filters';

@Injectable()
export class FiltersService {

  public updateParams(filters: IFilterParams = {}, key: string, value: string): IFilterParams {
    const filterKeys = Object.keys(filters);

    if (!value && !filterKeys.includes(key)) {
      return filters;
    }

    if (!value && filterKeys.includes(key)) {
      return this.removeParams(filters, key);
    }

    return this.addParams(filters, key, value);
  }

  private addParams(filters: IFilterParams, key: string, value: string): IFilterParams {
    return { ...filters, [key]: value };
  }

  private removeParams(filters: IFilterParams, key: string) {
    delete filters[key];
    return filters;
  }

}
