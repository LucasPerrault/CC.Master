import { ChangeDetectorRef, Component, forwardRef, Input } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { TranslatePipe } from '@cc/aspects/translate';
import { IEstablishment } from '@cc/domain/billing/establishments';

import { SelectDisplayMode } from '../select-display-mode.enum';

@Component({
  selector: 'cc-establishment-api-select',
  templateUrl: './establishment-api-select.component.html',providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => EstablishmentApiSelectComponent),
      multi: true,
    },
  ],
})
export class EstablishmentApiSelectComponent implements ControlValueAccessor {
  @Input() required = false;
  @Input() textfieldClass?: string;
  @Input() multiple = false;
  @Input() displayMode = SelectDisplayMode.Filter;

  public onChange: (d: IEstablishment | IEstablishment[]) => void;
  public onTouch: () => void;

  public apiUrl = 'api/v3/legalEntities';

  public model: IEstablishment | IEstablishment[] = [];
  public establishmentsSelected: IEstablishment[];

  public get filtersToExcludeSelection(): string[] {
    if (!this.multiple || !this.establishmentsSelected?.length) {
      return [];
    }

    const selectedIds = this.establishmentsSelected.map(e => e.id);
    return [`id=notequal,${selectedIds.join(',')}`];
  }

  constructor(private changeDetectorRef: ChangeDetectorRef, private translatePipe: TranslatePipe) {
  }

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(d: IEstablishment | IEstablishment[]): void {
    if (d !== this.model && d !== null) {
      this.model = d;
      this.changeDetectorRef.detectChanges();
    }
  }

  public update(d: IEstablishment | IEstablishment[]): void {
    this.onChange(d);
  }

  public setEstablishmentsDisplayed(): void {
    if (!this.multiple) {
      return;
    }

    this.establishmentsSelected = (this.model as IEstablishment[]);
  }

  public trackBy(index: number, establishment: IEstablishment): number {
    return establishment.id;
  }

  public get label(): string {
    const pluralCaseCount = 2;
    const singleCaseCount = 1;
    return this.translatePipe.transform('front_select_establishments_label', {
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
