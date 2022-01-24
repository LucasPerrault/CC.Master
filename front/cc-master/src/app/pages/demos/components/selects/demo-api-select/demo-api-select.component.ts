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
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { IDemo } from '../../../models/demo.interface';

@Component({
  selector: 'cc-demo-api-select',
  templateUrl: './demo-api-select.component.html',
  styleUrls: ['./demo-api-select.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => DemoApiSelectComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: DemoApiSelectComponent,
    },
  ],
})
export class DemoApiSelectComponent implements ControlValueAccessor, Validator, OnInit, OnDestroy {
  @Input() placeholder: string;
  @Input() required = false;

  public formControl: FormControl = new FormControl();

  public api = '/api/demos';
  // Todo : 21-02-2022 : Now LuApiPageSearcher doesn't have v4 sorting property. We need to add it in filters.
  public filters = ['sort=-isTemplate,subdomain'];

  private destroy$: Subject<void> = new Subject();

  public ngOnInit(): void {
    this.formControl.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(form => this.onChange(form));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (demo: IDemo) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(demo: IDemo): void {
    if (!!demo && demo !== this.formControl.value) {
      this.formControl.patchValue(demo);
    }
  }

  public validate(control: AbstractControl): ValidationErrors | null {
    if (this.formControl.invalid) {
      return { invalid: true };
    }
  }

  public trackBy(index: number, demo: IDemo): number {
    return demo.id;
  }
}
