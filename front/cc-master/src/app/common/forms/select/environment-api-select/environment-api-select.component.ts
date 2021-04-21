import { ChangeDetectorRef, Component, forwardRef, Input } from '@angular/core';
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
  @Input() public class?: string;

  public onChange: (environments: IEnvironment[]) => void;
  public onTouch: () => void;

  public apiUrl = '/api/v3/environments';
  public apiFields = 'id,subdomain';
  public apiOrderBy = 'subdomain,asc';

  public environmentsSelected: IEnvironment[];
  public environmentsSelectionDisplayed: IEnvironment[];

  public get filtersToExcludeSelection(): string[] {
    if (!this.environmentsSelectionDisplayed?.length) {
      return [];
    }

    const environmentSelectedIds = this.environmentsSelectionDisplayed.map(e => e.id);
    return [`id=notequal,${environmentSelectedIds.join(',')}`];
  }

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
    this.environmentsSelectionDisplayed = this.environmentsSelected;
  }

  public trackBy(index: number, environment: IEnvironment): number {
    return environment.id;
  }

  private reset(): void {
    this.onChange([]);
  }
}
