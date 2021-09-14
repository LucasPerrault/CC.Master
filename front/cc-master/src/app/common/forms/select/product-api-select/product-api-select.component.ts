import { ChangeDetectorRef, Component, forwardRef, Input } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { TranslatePipe } from '@cc/aspects/translate';
import { IProduct } from '@cc/domain/billing/offers';

import { SelectDisplayMode } from '../select-display-mode.enum';

@Component({
  selector: 'cc-product-api-select',
  templateUrl: './product-api-select.component.html',providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ProductApiSelectComponent),
      multi: true,
    },
  ],
})
export class ProductApiSelectComponent implements ControlValueAccessor {
  @Input() required = false;
  @Input() displayMode = SelectDisplayMode.Filter;
  @Input() textfieldClass?: string;
  @Input() multiple = false;

  @Input()
  public get disabled(): boolean { return this.isDisabled; }
  public set disabled(isDisabled: boolean) {
    this.setDisabledState(isDisabled);
  }

  public onChange: (d: IProduct | IProduct[]) => void;
  public onTouch: () => void;

  public apiUrl = 'api/v3/products';

  public model: IProduct | IProduct[] = [];
  public productsSelected: IProduct[];

  public get filtersToExcludeSelection(): string[] {
    if (!this.multiple || !this.productsSelected?.length) {
      return [];
    }

    const selectedIds = this.productsSelected.map(e => e.id);
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

  public writeValue(d: IProduct | IProduct[]): void {
    if (d !== this.model && d !== null) {
      this.model = d;
      this.changeDetectorRef.detectChanges();
    }
  }

  public setDisabledState(isDisabled: boolean) {
    this.isDisabled = isDisabled;
  }

  public update(d: IProduct | IProduct[]): void {
    this.onChange(d);
  }

  public setProductsDisplayed(): void {
    if (!this.multiple) {
      return;
    }

    this.productsSelected = (this.model as IProduct[]);
  }

  public trackBy(index: number, product: IProduct): number {
    return product.id;
  }

  public get label(): string {
    const pluralCaseCount = 2;
    const singleCaseCount = 1;
    return this.translatePipe.transform('front_select_products_label', {
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
