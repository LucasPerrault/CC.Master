import { Injectable } from '@angular/core';
import { IFilterParams } from '@cc/common/filters';

@Injectable()
export class FiltersService {

  public updateParam(filters: IFilterParams, key: string, value: string): IFilterParams {
    if (!key) {
      return filters;
    }

    return { ...filters, [key]: value };
  }

  public removeKey(filters: IFilterParams, key: string): IFilterParams {
    if (!key) {
      return filters;
    }

    delete filters[key];
    return filters;
  }

}
