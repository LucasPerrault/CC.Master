import { ChangeDetectionStrategy, Component, forwardRef, Input, OnDestroy, OnInit } from '@angular/core';
import { ControlValueAccessor, FormControl, NG_VALUE_ACCESSOR } from '@angular/forms';
import { TranslatePipe } from '@cc/aspects/translate';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { contractAdditionalColumns, IContractAdditionalColumn } from '../../constants/contract-additional-column.enum';

@Component({
  selector: 'cc-contract-additional-column-select',
  templateUrl: './contract-additional-column-select.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ContractAdditionalColumnSelectComponent),
      multi: true,
    },
  ],
})
export class ContractAdditionalColumnSelectComponent implements ControlValueAccessor, OnInit, OnDestroy {
  @Input() public textfieldClass: string;
  public onChange: (columns: IContractAdditionalColumn[]) => void;
  public onTouch: () => void;

  public formControl: FormControl = new FormControl();
  public get columns(): IContractAdditionalColumn[] {
    return this.getTranslations(contractAdditionalColumns);
  }

  private destroy$: Subject<void> = new Subject<void>();

  constructor(private translatePipe: TranslatePipe) { }

  public ngOnInit(): void {
    this.formControl.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(f => this.onChange(f));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(columns: IContractAdditionalColumn[]): void {
    if (!!columns && this.formControl.value !== columns) {
      this.formControl.patchValue(this.getTranslations(columns), { emitEvent: false });
    }
  }

  public getSelectionInRow(values: IContractAdditionalColumn[]): string {
    return values.map(v => v.name).join(', ');
  }

  public trackBy(index: number, column: IContractAdditionalColumn): string {
    return column.id;
  }

  private getTranslations(columns: IContractAdditionalColumn[]): IContractAdditionalColumn[] {
    return columns.map(c => ({ ...c, name: this.translatePipe.transform(c.name) }));
  }

}
