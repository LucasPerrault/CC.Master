import { ChangeDetectorRef, Component, forwardRef, Input } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { TranslatePipe } from '@cc/aspects/translate';
import { distributorFields, IDistributor } from '@cc/domain/billing/distributors';

import { SelectDisplayMode } from '../select-display-mode.enum';

@Component({
  selector: 'cc-distributor-api-select',
  templateUrl: './distributor-api-select.component.html',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => DistributorApiSelectComponent),
      multi: true,
    },
  ],
})
export class DistributorApiSelectComponent implements ControlValueAccessor {
  @Input() required = false;
  @Input() displayMode = SelectDisplayMode.Filter;
  @Input() textfieldClass?: string;
  @Input() multiple = false;

  @Input()
  public get disabled(): boolean { return this.isDisabled; }
  public set disabled(isDisabled: boolean) {
    this.setDisabledState(isDisabled);
  }

  public onChange: (d: IDistributor | IDistributor[]) => void;
  public onTouch: () => void;

  public apiUrl = 'api/v3/distributors';
  public fields = distributorFields;

  public model: IDistributor | IDistributor[] = [];
  public distributorsSelected: IDistributor[];

  public get filtersToExcludeSelection(): string[] {
    if (!this.multiple || !this.distributorsSelected?.length) {
      return [];
    }

    const selectedIds = this.distributorsSelected.map(e => e.id);
    return [`id=notequal,${selectedIds.join(',')}`];
  }

  private isDisabled = false;

  constructor(private changeDetectorRef: ChangeDetectorRef, private translatePipe: TranslatePipe) {
  }

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(d: IDistributor | IDistributor[]): void {
    if (d !== this.model && d !== null) {
      this.model = d;
      this.changeDetectorRef.detectChanges();
    }
  }

  public setDisabledState(isDisabled: boolean) {
    this.isDisabled = isDisabled;
  }

  public update(d: IDistributor | IDistributor[]): void {
    this.onChange(d);
  }

  public setDistributorsDisplayed(): void {
    if (!this.multiple) {
      return;
    }

    this.distributorsSelected = (this.model as IDistributor[]);
  }

  public trackBy(index: number, distributor: IDistributor): number {
    return distributor.id;
  }

  public get label(): string {
    const pluralCaseCount = 2;
    const singleCaseCount = 1;
    return this.translatePipe.transform('front_select_distributors_label', {
      count: this.multiple ? pluralCaseCount : singleCaseCount,
    });
  }

  public get placeholder(): string {
    if (this.isFormDisplayMode) {
      return;
    }

    return this.label;
  }

  public get isFormDisplayMode(): boolean {
    return this.displayMode === SelectDisplayMode.Form;
  }
}
