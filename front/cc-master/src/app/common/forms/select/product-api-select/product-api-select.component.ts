import { ChangeDetectorRef, Component, forwardRef, Input } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { TranslatePipe } from '@cc/aspects/translate';
import { IProduct } from '@cc/domain/billing/offers';

@Component({
  selector: 'cc-product-api-select',
  templateUrl: './product-api-select.component.html',
  styleUrls: ['./product-api-select.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ProductApiSelectComponent),
      multi: true,
    },
  ],
})
export class ProductApiSelectComponent implements ControlValueAccessor {
  @Input() required = false;
  @Input() multiple = false;
  @Input() placeholder: string;

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
}
