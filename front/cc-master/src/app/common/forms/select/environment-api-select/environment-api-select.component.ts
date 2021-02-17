import { Component, forwardRef } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { IEnvironment } from '@cc/domain/environments';
import { ALuApiService } from '@lucca-front/ng/api';

import { EnvironmentApiSelectService } from './environment-api-select.service';

@Component({
  selector: 'cc-environment-api-select',
  templateUrl: './environment-api-select.component.html',
  providers: [
    {
      provide: ALuApiService,
      useClass: EnvironmentApiSelectService,
    },
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => EnvironmentApiSelectComponent),
      multi: true,
    },
  ],
})
export class EnvironmentApiSelectComponent implements ControlValueAccessor {
  public onChange: (environmentIds: IEnvironment[]) => void;
  public onTouch: () => void;

  public apiUrl = '/api/v3/environments';
  public apiFields = 'id,subdomain';
  public apiOrderBy = 'subdomain,asc';

  public environmentIds: IEnvironment[];

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(environmentIdsSelected: IEnvironment[]): void {
    if (environmentIdsSelected !== this.environmentIds) {
      this.environmentIds = environmentIdsSelected;
    }
  }

  public safeOnChange(environmentIdsSelected: IEnvironment[]): void {
    if (!environmentIdsSelected) {
      this.reset();
      return;
    }

    this.onChange(environmentIdsSelected);
  }

  public trackBy(index: number, environment: IEnvironment): string {
    return environment.subDomain;
  }

  private reset(): void {
    this.onChange([]);
  }
}
