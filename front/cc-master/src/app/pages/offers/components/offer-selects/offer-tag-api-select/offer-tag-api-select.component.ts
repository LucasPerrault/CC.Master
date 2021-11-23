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

@Component({
  selector: 'cc-offer-tag-api-select',
  templateUrl: './offer-tag-api-select.component.html',
  styleUrls: ['offer-tag-api-select.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => OfferTagApiSelectComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: OfferTagApiSelectComponent,
    },
  ],
})
export class OfferTagApiSelectComponent implements OnInit, OnDestroy, ControlValueAccessor, Validator {
  @Input() public required = false;
  @Input() public hideClearer = false;
  @Input() public placeholder: string;
  @Input() public multiple: string;

  public api = '/api/commercial-offers/tags';

  public formControl: FormControl = new FormControl();
  private destroy$: Subject<void> = new Subject<void>();

  constructor() { }

  public ngOnInit(): void {
    this.formControl.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(tag => this.onChange(tag));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (tag: string) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(tag: string): void {
    if (!!tag && this.formControl.value !== tag) {
      this.formControl.setValue(tag);
    }
  }

  public validate(control: AbstractControl): ValidationErrors | null {
    if (this.formControl.invalid) {
      return { invalid: true };
    }
  }

  public trackBy(index: number, tag: string): string {
    return tag;
  }

  public capitalize(tag: string): string {
    return !!tag ? `${ tag[0].toUpperCase() }${ tag.slice(1) }` : '';
  }
}
