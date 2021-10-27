import { ChangeDetectorRef, Component, forwardRef, Input } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { TranslatePipe } from '@cc/aspects/translate';
import { IOffer } from '@cc/domain/billing/offers';

@Component({
  selector: 'cc-offer-api-select',
  templateUrl: './offer-api-select.component.html',
  styleUrls: ['./offer-api-select.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => OfferApiSelectComponent),
      multi: true,
    },
  ],
})
export class OfferApiSelectComponent implements ControlValueAccessor {
  @Input() required = false;
  @Input() multiple = false;
  @Input() placeholder: string;

  @Input()
  get filters(): string[] { return this.apiFilters; }
  set filters(filters: string[]) {
    this.apiFilters = [
      ...filters,
      ...this.filtersToExcludeSelection,
    ];
  }

  @Input()
  public get disabled(): boolean { return this.isDisabled; }
  public set disabled(isDisabled: boolean) {
    this.setDisabledState(isDisabled);
  }

  public get filtersToExcludeSelection(): string[] {
    if (!this.multiple || !this.offersSelected?.length) {
      return [];
    }

    const selectedIds = this.offersSelected.map(e => e.id);
    return [`id=notequal,${selectedIds.join(',')}`];
  }


  public onChange: (d: IOffer | IOffer[]) => void;
  public onTouch: () => void;

  public apiUrl = 'api/v3/offers';

  public model: IOffer | IOffer[] = [];
  public offersSelected: IOffer[];

  private apiFilters: string[];
  private isDisabled = false;

  constructor(private changeDetectorRef: ChangeDetectorRef, private translatePipe: TranslatePipe) {
  }

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(d: IOffer | IOffer[]): void {
    if (d !== this.model) {
      this.model = d;
      this.changeDetectorRef.detectChanges();
    }
  }

  public setDisabledState(isDisabled: boolean) {
    this.isDisabled = isDisabled;
  }

  public update(d: IOffer | IOffer[]): void {
    this.onChange(d);
  }

  public setOffersDisplayed(): void {
    if (!this.multiple) {
      return;
    }

    this.offersSelected = (this.model as IOffer[]);
  }

  public trackBy(index: number, offer: IOffer): number {
    return offer.id;
  }
}
