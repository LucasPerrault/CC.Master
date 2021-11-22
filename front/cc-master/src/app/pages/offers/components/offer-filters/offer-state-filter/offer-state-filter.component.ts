import { ChangeDetectionStrategy, Component, forwardRef, OnDestroy, OnInit } from '@angular/core';
import { ControlValueAccessor, FormControl, NG_VALUE_ACCESSOR } from '@angular/forms';
import { TranslatePipe } from '@cc/aspects/translate';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { getOfferState, OfferState } from './offer-state-filter.interface';

@Component({
  selector: 'cc-offer-state-filter',
  templateUrl: './offer-state-filter.component.html',
  styleUrls: ['./offer-state-filter.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => OfferStateFilterComponent),
      multi: true,
    },
  ],
})
export class OfferStateFilterComponent implements OnInit, OnDestroy, ControlValueAccessor {
  public state = OfferState;
  public formControl: FormControl = new FormControl();

  private destroy$: Subject<void> = new Subject<void>();

  constructor(private translatePipe: TranslatePipe) {
  }

  public ngOnInit(): void {
    this.formControl.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(billingMode => this.onChange(billingMode));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (state: OfferState) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(state: OfferState): void {
    if (!!state && this.formControl.value !== state) {
      console.log('state', state);
      this.formControl.setValue(state);
    }
  }

  public getStateName(state: OfferState): string {
    const translationKey = getOfferState(state).name;
    return !!translationKey ? this.translatePipe.transform(translationKey) : '';
  }
}
