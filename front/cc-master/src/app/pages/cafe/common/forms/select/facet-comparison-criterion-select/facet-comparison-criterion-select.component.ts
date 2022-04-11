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
import { FormlyFieldConfig } from '@ngx-formly/core/lib/components/formly.field.config';
import { Observable, pipe, Subject, UnaryFunction } from 'rxjs';
import { map, takeUntil } from 'rxjs/operators';

import { IFacetComparisonCriterion } from '../../../components/advanced-filter-form';
import { FacetScope, IFacet } from '../../../models';

@Component({
  selector: 'cc-facet-comparison-criterion-select',
  templateUrl: './facet-comparison-criterion-select.component.html',
  styleUrls: ['./facet-comparison-criterion-select.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => FacetComparisonCriterionSelectComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: FacetComparisonCriterionSelectComponent,
    },
  ],
})
export class FacetComparisonCriterionSelectComponent implements OnInit, OnDestroy, ControlValueAccessor, Validator {
  @Input() facetScope: FacetScope;
  @Input() placeholder: string;
  @Input() multiple = false;
  @Input() required = false;
  @Input() formlyAttributes: FormlyFieldConfig = {};

  public formControl: FormControl = new FormControl();

  private destroy$: Subject<void> = new Subject();

  public ngOnInit(): void {
    this.formControl.valueChanges
      .pipe(takeUntil(this.destroy$), this.toComparisonCriterion)
      .subscribe(criterion => this.onChange(criterion));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (criterion: IFacetComparisonCriterion) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(criterion: IFacetComparisonCriterion): void {
    if (!!criterion && criterion !== this.formControl.value) {
      this.formControl.setValue(criterion.facet);
    }
  }

  public validate(control: AbstractControl): ValidationErrors | null {
    if (this.formControl.invalid) {
      return  { invalid: true };
    }
  }

  public get toComparisonCriterion(): UnaryFunction<Observable<IFacet>, Observable<IFacetComparisonCriterion>> {
    return pipe(map(facet => ({
      key: facet.type,
      name: facet.type,
      facet,
    })));
  }
}
