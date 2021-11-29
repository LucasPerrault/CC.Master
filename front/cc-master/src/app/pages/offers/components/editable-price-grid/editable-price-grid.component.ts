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
import { BehaviorSubject, Subject } from 'rxjs';
import { filter, takeUntil } from 'rxjs/operators';

import { IPriceRowForm } from '../../models/price-list-form.interface';
import { PriceListsValidators, PriceListValidationError } from '../../services/price-lists.validators';
import { IPriceListOfferSelectOption } from '../offer-selects/offer-price-list-api-select/offer-price-list-selection.interface';

enum PriceRowFormKey {
  Id = 'id',
  MaxIncludedCount = 'maxIncludedCount',
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
  @Input() public withAddPriceRowButton = false;
  @Input() public set readonly(isReadonly: boolean) { this.readonly$.next(isReadonly); }
  @ViewChild('tableElement') public tableElement: ElementRef<HTMLTableElement>;

  public readonly$: BehaviorSubject<boolean> = new BehaviorSubject(false);
  public readonlyFormControls: AbstractControl[] = [];

  public get canRemove(): boolean {
    return this.formArray.length > 1;
  }

  public duplicatedOffer: FormControl = new FormControl();

  public formArrayKey = 'rows';
  public formArray: FormArray = new FormArray([]);
  public formKey = PriceRowFormKey;
  public formGroup: FormGroup = new FormGroup(
    { [this.formArrayKey]: this.formArray },
    [PriceListsValidators.boundsContinuity, PriceListsValidators.validPrices, PriceListsValidators.required],
  );

  public validationError = PriceListValidationError;
  public get hasFormErrors(): boolean {
    const hasRequiredError = this.formGroup.dirty && this.formGroup.hasError(PriceListValidationError.Required);
    return hasRequiredError
      || this.formGroup.hasError(PriceListValidationError.BoundsContinuity)
      || this.formGroup.hasError(PriceListValidationError.ValidPrices);
  }

  private destroy$: Subject<void> = new Subject<void>();

  constructor() { }

  public ngOnInit(): void {
    this.formGroup.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(formGroup => this.onChange(formGroup[this.formArrayKey]));

    this.readonly$
      .pipe(takeUntil(this.destroy$))
      .subscribe(readonly => this.setReadOnlyState(readonly));

    this.duplicatedOffer.valueChanges
      .pipe(takeUntil(this.destroy$), filter(o => !!o))
      .subscribe((offer: IPriceListOfferSelectOption) => this.reset(offer.priceList.rows));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (priceRows: IPriceRowForm[]) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(priceRows: IPriceRowForm[]): void {
    if (!priceRows?.length) {
      this.init();
      return;
    }

    if (!!priceRows) {
      this.reset(priceRows);
    }
  }

  public validate(control: AbstractControl): ValidationErrors | null {
    if (this.formGroup.invalid) {
      return  { invalid: true };
    }
  }

  public insert(): void {
    const lastIndex = this.formArray.length - 1;
    const priceRow: IPriceRowForm = this.formArray.at(lastIndex).value;
    const nextUpperBound = priceRow.maxIncludedCount + 1;
    const formGroup = this.getFormGroup(nextUpperBound);

    this.formArray.insert(lastIndex + 1, formGroup);
  }

  public addRange(priceRows?: IPriceRowForm[]): void {
    priceRows.forEach(priceRow => this.add(priceRow));
  }

  public remove(index: number) {
    this.formArray.removeAt(index);
  }

  public reset(priceRows: IPriceRowForm[]): void {
    this.formArray.clear();
    this.addRange(priceRows);
    this.updateReadonlyState();
  }

  public paste(event: ClipboardEvent): void {
    this.formArray.clear();

    const csv = event.clipboardData.getData('text');
    const rows = csv.split('\n');
    const validRows = rows.filter(row => this.isPriceRowValid(row));
    const priceRows = validRows.map(row => this.toPriceRow(row));

    this.addRange(priceRows);
  }

  public updateFocus(arrow: ArrowKey, row: number, col: number): void {
    const nextFocusedPosition = this.getNextFocusedPosition(arrow, row, col);

    const tableElementRowIndex = nextFocusedPosition.row + 1;
    const tableRow = this.tableElement.nativeElement.rows.item(tableElementRowIndex);
    const tableCell = tableRow.cells.item(nextFocusedPosition.col);
    const label = tableCell.childNodes.item(0);
    const input = label.childNodes.item(0) as HTMLInputElement;
    input?.focus();
  }

  public getLowerBound(maxIncludedCount: number): number {
    const priceRows = this.formArray.value;
    const currentRowIndex = priceRows.findIndex(row => row.maxIncludedCount === maxIncludedCount);
    const previousMaxIncludedCount = priceRows[currentRowIndex - 1]?.maxIncludedCount;

    return !!previousMaxIncludedCount ? previousMaxIncludedCount + 1 : 0;
  }

  public isReadonly(control: AbstractControl): boolean {
    return !!this.readonlyFormControls.find(c => c === control);
  }

  private updateReadonlyState(): void {
    this.setReadOnlyState(this.readonly$.value);
  }

  private setReadOnlyState(isReadonly: boolean): void {
    if (isReadonly) {
      this.readonlyFormControls.push(...this.formArray.controls);
      return;
    }
    this.readonlyFormControls = [];
  }

  private init(): void {
    this.add();
    this.updateReadonlyState();
  }

  private add(priceRow?: IPriceRowForm): void {
    const formGroup = this.getFormGroup(priceRow?.maxIncludedCount, priceRow?.unitPrice, priceRow?.fixedPrice, priceRow.id);
    this.formArray.push(formGroup);
  }

  private getFormGroup(maxIncludedCount: number, unitPrice?: number, fixedPrice?: number, id?: number): FormGroup {
    return new FormGroup(
      {
        [PriceRowFormKey.Id]: new FormControl(id),
        [PriceRowFormKey.MaxIncludedCount]: new FormControl(maxIncludedCount),
        [PriceRowFormKey.UnitPrice]: new FormControl(unitPrice),
        [PriceRowFormKey.FixedPrice]: new FormControl(fixedPrice),
      },
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

  private toPriceRow(row: string): IPriceRowForm {
    const columns = row.split('\t');

    return {
      maxIncludedCount: parseInt(columns[1], 10),
      unitPrice: parseInt(columns[2], 10),
      fixedPrice: parseInt(columns[3], 10),
    };
  }
}
