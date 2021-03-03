import { ChangeDetectorRef, Component, forwardRef } from '@angular/core';
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
  public onChange: (environments: IEnvironment[]) => void;
  public onTouch: () => void;

  public apiUrl = '/api/v3/environments';
  public apiFields = 'id,subdomain';
  public apiOrderBy = 'subdomain,asc';
  public apiFilters = [];

  public environmentsSelected: IEnvironment[];
  public environmentsSelectionDisplayed: IEnvironment[];

  constructor(private changeDetectorRef: ChangeDetectorRef) {
  }

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(environmentsSelected: IEnvironment[]): void {
    if (environmentsSelected !== this.environmentsSelected) {
      this.environmentsSelected = environmentsSelected;
      this.changeDetectorRef.detectChanges();
    }
  }

  public safeOnChange(environmentsSelected: IEnvironment[]): void {
    if (!environmentsSelected) {
      this.reset();
      return;
    }

    this.onChange(environmentsSelected);
  }

  public setEnvironmentsDisplayed(): void {
    if (!this.environmentsSelected || !this.environmentsSelected.length) {
      this.resetEnvironmentsDisplayed();
      return;
    }

    this.environmentsSelectionDisplayed = this.environmentsSelected;
    const environmentSelectedIds = this.environmentsSelected.map(e => e.id);
    this.apiFilters = [`id=notequal,${environmentSelectedIds.join(',')}`];
  }

  public trackBy(index: number, environment: IEnvironment): number {
    return environment.id;
  }

  private reset(): void {
    this.onChange([]);
  }

  private resetEnvironmentsDisplayed(): void {
    this.environmentsSelectionDisplayed = [];
    this.apiFilters = [];
  }
}
