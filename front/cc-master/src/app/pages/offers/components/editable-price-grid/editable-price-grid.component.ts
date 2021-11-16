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
import { IOffer } from '@cc/domain/billing/offers';
import { BehaviorSubject, Subject } from 'rxjs';
import { filter, finalize, map, switchMap, takeUntil, tap } from 'rxjs/operators';

import { IPriceRowForm } from '../../models/price-list-form.interface';
import { PriceListsTimelineService } from '../../services/price-lists-timeline.service';
import { OffersDataService } from '../../services/offers-data.service';
import { EditablePriceGridValidators, PriceGridValidationError } from './editable-price-grid.validators';

enum PriceRowFormKey {
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
  @Input() public set disabled(isDisabled: boolean) { this.setDisabledState(isDisabled); }
  @ViewChild('tableElement') public tableElement: ElementRef<HTMLTableElement>;

  public get canRemove(): boolean {
    return this.formArray.length > 1;
  }

  public offer: FormControl = new FormControl();
  public isPriceListsLoading$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  public formArrayKey = 'priceRows';
  public formArray: FormArray = new FormArray([]);
  public formKey = PriceRowFormKey;
  public formGroup: FormGroup = new FormGroup(
    { [this.formArrayKey]: this.formArray },
    [EditablePriceGridValidators.boundsContinuity],
  );

  public validationError = PriceGridValidationError;
  public get hasFormErrors(): boolean {
    return this.hasRequiredError || this.formGroup.hasError(PriceGridValidationError.BoundsContinuity);
  }

  public get hasRequiredError(): boolean {
    return !!this.formArray.controls.find((c: FormGroup) => {
      const priceRowKeys = Object.keys(c.controls);
      return !!priceRowKeys.find(key => c.get(key).hasError(PriceGridValidationError.Required));
    });
  }

  private destroy$: Subject<void> = new Subject<void>();

  constructor(private dataService: OffersDataService) { }

  public ngOnInit(): void {
    this.formGroup.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(formGroup => this.onChange(formGroup[this.formArrayKey]));

    this.offer.valueChanges
      .pipe(
        takeUntil(this.destroy$), filter(o => !!o),
        tap(() => this.isPriceListsLoading$.next(true)),
        switchMap((o: IOffer) => this.dataService.getPriceLists$(o.id)
          .pipe(finalize(() => this.isPriceListsLoading$.next(false)))),
        map(priceLists => PriceListsTimelineService.getCurrent(priceLists)),
      )
      .subscribe(priceList => this.reset(priceList.rows));
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

  public setDisabledState(isDisabled: boolean) {
    if (isDisabled) {
      this.offer.disable();
      this.formArray.controls.forEach(control => control.disable());
      return;
    }
    this.offer.enable();
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
    const formGroup = this.getFormGroup(nextUpperBound, 0, 0);

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

  private init(): void {
    const defaultPriceRow: IPriceRowForm = { maxIncludedCount: 0, unitPrice: 0, fixedPrice: 0 };
    this.add(defaultPriceRow);
  }

  private add(priceRow: IPriceRowForm): void {
    const formGroup = this.getFormGroup(priceRow.maxIncludedCount, priceRow.unitPrice, priceRow.fixedPrice);
    this.formArray.push(formGroup);
  }

  private getFormGroup(maxIncludedCount: number, unitPrice?: number, fixedPrice?: number): FormGroup {
    return new FormGroup(
      {
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
