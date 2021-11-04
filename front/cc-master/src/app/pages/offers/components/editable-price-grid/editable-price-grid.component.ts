import { ChangeDetectionStrategy, Component, ElementRef, forwardRef, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import {
  AbstractControl,
  ControlValueAccessor,
  FormArray,
  FormControl,
  FormGroup,
  NG_VALIDATORS,
  NG_VALUE_ACCESSOR,
  ValidationErrors,
  Validator,
} from '@angular/forms';
import { IPriceList } from '@cc/domain/billing/offers';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { IOfferCurrency } from '../../models/offer-currency.interface';
import { IEditablePriceGrid } from './editable-price-grid.interface';
import { EditablePriceGridValidators, PriceGridValidationError } from './editable-price-grid.validators';

enum PriceListFormKey {
  LowerBound = 'lowerBound',
  UpperBound = 'upperBound',
  UnitPrice = 'unitPrice',
  FixedPrice = 'fixedPrice',
}

interface ITablePosition {
  row: number;
  col: number;
}

export enum ArrowKey {
  Up = 'ArrowUp',
  Down = 'ArrowDown',
  Left = 'ArrowLeft',
  Right = 'ArrowRight',
}

@Component({
  selector: 'cc-editable-price-grid',
  templateUrl: './editable-price-grid.component.html',
  styleUrls: ['./editable-price-grid.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => EditablePriceGridComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: EditablePriceGridComponent,
    },
  ],
})
export class EditablePriceGridComponent implements OnInit, OnDestroy, ControlValueAccessor, Validator {
  @ViewChild('tableElement') public tableElement: ElementRef<HTMLTableElement>;

  @Input() public currency: IOfferCurrency;
  @Input() public set priceLists(priceLists: IPriceList[]) {
    this.addRange(priceLists);
  }

  public get canRemove(): boolean {
    return this.formArray.length > 1;
  }

  public formArrayKey = 'priceLists';
  public formArray: FormArray = new FormArray([]);
  public formKey = PriceListFormKey;
  public formGroup: FormGroup = new FormGroup(
    { [this.formArrayKey]: this.formArray },
    [EditablePriceGridValidators.boundsContinuity, EditablePriceGridValidators.minimumZero],
  );

  public validationError = PriceGridValidationError;
  public get hasFormErrors(): boolean {
    return this.hasClosestBoundsError
      || this.hasRequiredError
      || this.formGroup.hasError(PriceGridValidationError.BoundsContinuity)
      || this.formGroup.hasError(PriceGridValidationError.MinBound);
  }

  public get hasClosestBoundsError(): boolean {
    return !!this.formArray.controls.find(c => c.hasError(PriceGridValidationError.ClosestBounds));
  }

  public get hasRequiredError(): boolean {
    return !!this.formArray.controls.find((c: FormGroup) => {
      const priceListKeys = Object.keys(c.controls);
      return !!priceListKeys.find(key => c.get(key).hasError(PriceGridValidationError.Required));
    });
  }

  private destroy$: Subject<void> = new Subject<void>();

  constructor() { }

  public ngOnInit(): void {
    this.formGroup.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(formGroup => this.onChange(formGroup[this.formArrayKey]));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (priceLists: IEditablePriceGrid[]) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(priceLists: IEditablePriceGrid[]): void {
    if (!priceLists?.length) {
      this.init();
      return;
    }

    if (!!priceLists) {
      this.formArray.clear();
      this.addRange(priceLists);
    }
  }

  public validate(control: AbstractControl): ValidationErrors | null {
    if (this.formGroup.invalid) {
      return  { invalid: true };
    }
  }

  public insert(): void {
    const lastIndex = this.formArray.length - 1;
    const priceList: IEditablePriceGrid = this.formArray.at(lastIndex).value;
    const nextLowerBound = priceList.upperBound + 1;
    const formGroup = this.getFormGroup(nextLowerBound, nextLowerBound, 0, 0);

    this.formArray.insert(lastIndex + 1, formGroup);
  }

  public addRange(priceLists?: IEditablePriceGrid[]): void {
    priceLists.forEach(priceList => this.add(priceList));
  }

  public remove(index: number) {
    this.formArray.removeAt(index);
  }

  public paste(event: ClipboardEvent): void {
    this.formArray.clear();

    const csv = event.clipboardData.getData('text');
    const rows = csv.split('\n');
    const validRows = rows.filter(row => this.isPriceRowValid(row));
    const priceRows = validRows.map(row => this.toPriceList(row));

    this.addRange(priceRows);
  }

  public updateFocus(arrow: ArrowKey, row: number, col: number): void {
    const nextFocusedPosition = this.getNextFocusedPosition(arrow, row, col);

    const tableElementRowIndex = nextFocusedPosition.row + 1;
    const tableRow = this.tableElement.nativeElement.rows.item(tableElementRowIndex);
    const tableCell = tableRow.cells.item(nextFocusedPosition.col);
    const label = tableCell.childNodes.item(0);
    const input = label.childNodes.item(0) as HTMLInputElement;
    input.focus();
  }

  private init(): void {
    const defaultPriceList: IEditablePriceGrid = { lowerBound: 0, upperBound: 0, unitPrice: 0, fixedPrice: 0 };
    this.add(defaultPriceList);
  }

  private add(priceList: IEditablePriceGrid): void {
    const formGroup = this.getFormGroup(priceList.lowerBound, priceList.upperBound, priceList.unitPrice, priceList.fixedPrice);
    this.formArray.push(formGroup);
  }

  private getFormGroup(lowerBound?: number, upperBound?: number, unitPrice?: number, fixedPrice?: number): FormGroup {
    return new FormGroup(
      {
        [PriceListFormKey.LowerBound]: new FormControl(lowerBound),
        [PriceListFormKey.UpperBound]: new FormControl(upperBound),
        [PriceListFormKey.UnitPrice]: new FormControl(unitPrice),
        [PriceListFormKey.FixedPrice]: new FormControl(fixedPrice),
      },
      EditablePriceGridValidators.upperBoundSuperior,
    );
  }

  private getNextFocusedPosition(arrow: ArrowKey, row: number, col: number): ITablePosition {
    const leftBorder = 0;
    const rightBorder = 3;
    const topBorder = 0;
    const bottomBorder = this.formArray.length - 1;

    switch (arrow) {
      case ArrowKey.Up:
        return { row: row === topBorder ? bottomBorder : row - 1, col };
      case ArrowKey.Down:
        return { row: row === bottomBorder ? topBorder : row + 1 , col };
      case ArrowKey.Right:
        return { row, col: col === rightBorder ? leftBorder : col + 1 };
      case ArrowKey.Left:
        return { row, col: col === leftBorder ? rightBorder : col - 1 };
    }
  }

  private isPriceRowValid(row: string): boolean {
    const requiredColumnsNumber = 4;
    const columns = row.split('\t');
    return columns.length === requiredColumnsNumber;
  }

  private toPriceList(row: string): IEditablePriceGrid {
    const columns = row.split('\t');
    return {
      lowerBound: parseInt(columns[0], 10),
      upperBound: parseInt(columns[1], 10),
      unitPrice: parseInt(columns[2], 10),
      fixedPrice: parseInt(columns[3], 10),
    };
  }
}
