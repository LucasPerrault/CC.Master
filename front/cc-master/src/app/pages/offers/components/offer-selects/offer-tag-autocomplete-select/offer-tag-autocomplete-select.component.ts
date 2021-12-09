import { ChangeDetectionStrategy, Component, forwardRef, Input, OnDestroy, OnInit } from '@angular/core';
import { ControlValueAccessor, FormControl, NG_VALUE_ACCESSOR } from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'cc-offer-tag-autocomplete-select',
  templateUrl: './offer-tag-autocomplete-select.component.html',
  styleUrls: ['offer-tag-autocomplete-select.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => OfferTagAutocompleteSelectComponent),
      multi: true,
    },
  ],
})
export class OfferTagAutocompleteSelectComponent implements OnInit, OnDestroy, ControlValueAccessor {
  @Input() public required = false;
  @Input() public placeholder: string;
  @Input() public multiple: string;
  @Input() public set disabled(isDisabled: boolean) { this.setDisabledState(isDisabled); }

  public api = '/api/commercial-offers/tags';

  public autocomplete: FormControl = new FormControl();

  public formControl: FormControl = new FormControl();
  private destroy$: Subject<void> = new Subject<void>();

  constructor() { }

  public ngOnInit(): void {
    this.formControl.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(tag => this.onChange(tag));

    this.autocomplete.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(autocomplete => this.formControl.setValue(autocomplete));
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

  public setDisabledState(isDisabled: boolean) {
    if (isDisabled) {
      this.formControl.disable();
      return;
    }
    this.formControl.enable();
  }

  public trackBy(index: number, tag: string): string {
    return tag;
  }

  public capitalize(tag: string): string {
    return !!tag ? `${ tag[0].toUpperCase() }${ tag.slice(1) }` : '';
  }
}
