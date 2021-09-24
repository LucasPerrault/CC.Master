import { ChangeDetectionStrategy, Component, forwardRef, Input, OnDestroy, OnInit } from '@angular/core';
import { ControlValueAccessor, FormControl, NG_VALUE_ACCESSOR } from '@angular/forms';
import { ALuApiService } from '@lucca-front/ng/api';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { OfferTagApiSelectService } from './offer-tag-api-select.service';

@Component({
  selector: 'cc-offer-tag-api-select',
  templateUrl: './offer-tag-api-select.component.html',
  styleUrls: ['offer-tag-api-select.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: ALuApiService,
      useClass: OfferTagApiSelectService,
    },
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => OfferTagApiSelectComponent),
      multi: true,
    },
  ],
})
export class OfferTagApiSelectComponent implements OnInit, OnDestroy, ControlValueAccessor {
  @Input() public placeholder: string;
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

  public trackBy(index: number, tag: string): string {
    return tag;
  }
}
