import { ChangeDetectionStrategy, Component, forwardRef, Input, OnDestroy, OnInit } from '@angular/core';
import { ControlValueAccessor, FormControl, NG_VALUE_ACCESSOR } from '@angular/forms';
import { TranslatePipe } from '@cc/aspects/translate';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { environmentAdditionalColumns, IEnvironmentAdditionalColumn } from './environment-additional-column.enum';

@Component({
  selector: 'cc-environment-additional-column-select',
  templateUrl: './environment-additional-column-select.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => EnvironmentAdditionalColumnSelectComponent),
      multi: true,
    },
  ],
})
export class EnvironmentAdditionalColumnSelectComponent implements ControlValueAccessor, OnInit, OnDestroy {
  @Input() public textfieldClass: string;
  public onChange: (columns: IEnvironmentAdditionalColumn[]) => void;
  public onTouch: () => void;

  public formControl: FormControl = new FormControl();
  public get columns(): IEnvironmentAdditionalColumn[] {
    return this.getTranslations(environmentAdditionalColumns);
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

  public writeValue(columns: IEnvironmentAdditionalColumn[]): void {
    if (!!columns && this.formControl.value !== columns) {
      this.formControl.patchValue(this.getTranslations(columns), { emitEvent: false });
    }
  }

  public getSelectionInRow(values: IEnvironmentAdditionalColumn[]): string {
    return values.map(v => v.name).join(', ');
  }

  public trackBy(index: number, column: IEnvironmentAdditionalColumn): string {
    return column.id;
  }

  private getTranslations(columns: IEnvironmentAdditionalColumn[]): IEnvironmentAdditionalColumn[] {
    return columns.map(c => ({ ...c, name: this.translatePipe.transform(c.name) }));
  }

}
