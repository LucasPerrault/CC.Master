import { ChangeDetectionStrategy, Component, forwardRef, Input, OnDestroy, OnInit } from '@angular/core';
import { ControlValueAccessor, FormControl, NG_VALUE_ACCESSOR } from '@angular/forms';
import { TranslatePipe } from '@cc/aspects/translate';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { countAdditionalColumns, ICountAdditionalColumn } from './count-additional-column.enum';

@Component({
  selector: 'cc-count-additional-column-select',
  templateUrl: './count-additional-column-select.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => CountAdditionalColumnSelectComponent),
      multi: true,
    },
  ],
})
export class CountAdditionalColumnSelectComponent implements ControlValueAccessor, OnInit, OnDestroy {
  @Input() public textfieldClass: string;

  public formControl: FormControl = new FormControl();
  public get columns(): ICountAdditionalColumn[] {
    return this.getTranslations(countAdditionalColumns);
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

  public onChange: (columns: ICountAdditionalColumn[]) => void = () => {};
  public onTouch: () => void = () => {};


  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(columns: ICountAdditionalColumn[]): void {
    if (!!columns && this.formControl.value !== columns) {
      this.formControl.patchValue(this.getTranslations(columns));
    }
  }

  public getSelectionInRow(values: ICountAdditionalColumn[]): string {
    return values.map(v => v.name).join(', ');
  }

  public trackBy(index: number, column: ICountAdditionalColumn): string {
    return column.id;
  }

  private getTranslations(columns: ICountAdditionalColumn[]): ICountAdditionalColumn[] {
    return columns.map(c => ({ ...c, name: this.translatePipe.transform(c.name) }));
  }

}
