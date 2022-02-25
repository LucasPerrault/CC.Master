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
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { FacetScope } from '../../../models';
import { FacetValue } from '../../../models/facet-value.interface';

@Component({
  selector: 'cc-facet-value-api-select',
  templateUrl: './facet-value-api-select.component.html',
  styleUrls: ['./facet-value-api-select.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => FacetValueApiSelectComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: FacetValueApiSelectComponent,
    },
  ],
})
export class FacetValueApiSelectComponent implements ControlValueAccessor, Validator, OnInit, OnDestroy {
  @Input() facetScope: FacetScope;
  @Input() placeholder: string;
  @Input() multiple = false;
  @Input() required = false;
  @Input() filters: string[];
  @Input() formlyAttributes: FormlyFieldConfig = {};

  public get api(): string {
    switch (this.facetScope) {
      case FacetScope.Environment:
        return '/api/cafe/facets/environments/values';
      case FacetScope.Establishment:
        return '/api/cafe/facets/establishments/values';
    }
  }

  public formControl: FormControl = new FormControl();

  private destroy$: Subject<void> = new Subject();

  public ngOnInit(): void {
    this.formControl.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(facet => this.onChange(facet));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (facet: FacetValue | FacetValue[]) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(facet: FacetValue | FacetValue[]): void {
    if (!!facet && facet !== this.formControl.value) {
      this.formControl.patchValue(facet);
    }
  }

  public validate(control: AbstractControl): ValidationErrors | null {
    if (this.formControl.invalid) {
      return { invalid: true };
    }
  }

  public trackBy(index: number, facetValue: FacetValue): FacetValue {
    return facetValue;
  }

  public getFacetValue(facetValue: FacetValue): string {
    return facetValue.value;
  }
}
