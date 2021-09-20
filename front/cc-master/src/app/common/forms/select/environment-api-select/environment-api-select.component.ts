import { ChangeDetectorRef, Component, forwardRef, Input } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { TranslatePipe } from '@cc/aspects/translate';
import { IEnvironment } from '@cc/domain/environments';
import { ALuApiService } from '@lucca-front/ng/api';

import { SelectDisplayMode } from '../../select/select-display-mode.enum';
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
  @Input() public textfieldClass?: string;
  @Input() multiple = false;
  @Input() required = false;
  @Input() displayMode = SelectDisplayMode.Filter;


  public onChange: (e: IEnvironment[] | IEnvironment) => void;
  public onTouch: () => void;

  public apiUrl = '/api/v3/environments';
  public apiFields = 'id,subdomain';
  public apiOrderBy = 'subdomain,asc';

  public environmentsSelected: IEnvironment | IEnvironment[] = [];
  public environmentsSelectionDisplayed: IEnvironment[];

  public get filtersToExcludeSelection(): string[] {
    if (!this.multiple || !this.environmentsSelectionDisplayed?.length) {
      return [];
    }

    const environmentSelectedIds = this.environmentsSelectionDisplayed.map(e => e.id);
    return [`id=notequal,${environmentSelectedIds.join(',')}`];
  }

  constructor(private changeDetectorRef: ChangeDetectorRef, private translatePipe: TranslatePipe) {
  }

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(e: IEnvironment | IEnvironment[]): void {
    if (e !== this.environmentsSelected  && e !== null) {
      this.environmentsSelected = e;
      this.changeDetectorRef.detectChanges();
    }
  }

  public update(e: IEnvironment | IEnvironment[]): void {
    this.onChange(e);
  }

  public setEnvironmentsDisplayed(): void {
    if (!this.multiple) {
      return;
    }

    this.environmentsSelectionDisplayed = (this.environmentsSelected as IEnvironment[]);
  }

  public trackBy(index: number, environment: IEnvironment): number {
    return environment.id;
  }

  public get label(): string {
    const pluralCaseCount = 2;
    const singleCaseCount = 1;
    return this.translatePipe.transform('front_select_environments_label', {
      count: this.multiple ? pluralCaseCount : singleCaseCount,
    });
  }

  public get isFormDisplayMode(): boolean {
    return this.displayMode === SelectDisplayMode.Form;
  }

  public get placeholder(): string {
    if (this.isFormDisplayMode) {
      return;
    }

    return this.label;
  }
}
