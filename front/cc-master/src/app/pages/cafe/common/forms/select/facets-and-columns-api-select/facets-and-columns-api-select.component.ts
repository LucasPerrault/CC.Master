import { ChangeDetectionStrategy, Component, forwardRef, Input, OnDestroy, OnInit } from '@angular/core';
import {
  AbstractControl,
  ControlValueAccessor,
  FormControl,
  NG_VALIDATORS,
  NG_VALUE_ACCESSOR,
  ValidationErrors,
  Validator,
} from '@angular/forms';
import { TranslatePipe } from '@cc/aspects/translate';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { IFacetAndColumn } from './facet-and-column.interface';

@Component({
  selector: 'cc-facets-and-columns-api-select',
  templateUrl: './facets-and-columns-api-select.component.html',
  styleUrls: ['./facets-and-columns-api-select.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => FacetsAndColumnsApiSelectComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: FacetsAndColumnsApiSelectComponent,
    },
  ],
})
export class FacetsAndColumnsApiSelectComponent implements ControlValueAccessor, Validator, OnInit, OnDestroy {
  @Input() placeholder: string;

  public formControl: FormControl = new FormControl();
  private destroy$: Subject<void> = new Subject();

  constructor(private translatePipe: TranslatePipe) {}

  public ngOnInit(): void {
    this.formControl.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(facetsAndColumns => this.onChange(facetsAndColumns));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (facetsAndColumns: IFacetAndColumn[]) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(facetsAndColumns: IFacetAndColumn[]): void {
    console.log(facetsAndColumns);
    if (!!facetsAndColumns && facetsAndColumns !== this.formControl.value) {
      const translated = facetsAndColumns.map(f => ({ ...f, name: this.translatePipe.transform(f?.name) }));
      this.formControl.patchValue(translated);
    }
  }

  public validate(control: AbstractControl): ValidationErrors | null {
    if (this.formControl.invalid) {
      return { invalid: true };
    }
  }

  public trackBy(index: number, facetsAndColumns: IFacetAndColumn): string | number {
    return facetsAndColumns.id;
  }

  public compareWithFn(option1: IFacetAndColumn, option2: IFacetAndColumn): boolean {
    return option1.id === option2.id;
  }
}
