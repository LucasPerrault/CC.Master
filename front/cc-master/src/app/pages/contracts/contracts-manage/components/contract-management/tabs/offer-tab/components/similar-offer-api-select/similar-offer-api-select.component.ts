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
import { ApiV3DateService } from '@cc/common/queries';
import { IOffer } from '@cc/domain/billing/offers';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { ISimilarOfferContext } from '../../models/similar-offer-context.interface';

@Component({
  selector: 'cc-similar-offer-api-select',
  templateUrl: './similar-offer-api-select.component.html',
  styleUrls: ['./similar-offer-api-select.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => SimilarOfferApiSelectComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: SimilarOfferApiSelectComponent,
    },
  ],
})
export class SimilarOfferApiSelectComponent implements ControlValueAccessor, Validator, OnInit, OnDestroy {
  @Input() placeholder: string;
  @Input() required = false;
  @Input() context: ISimilarOfferContext;

  public get api(): string {
    return !!this.context?.offerId ? `/api/commercial-offers/${ this.context.offerId }/similar` : '';
  }

  public get filters(): string[] {
    return !!this.context?.maxPeriod ? [`until=${ this.apiDateService.toApiV3DateFormat(this.context.maxPeriod) }`] : [];
  }

  public formControl: FormControl = new FormControl();

  private destroy$: Subject<void> = new Subject();

  constructor(private apiDateService: ApiV3DateService) {}

  public ngOnInit(): void {
    this.formControl.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(form => this.onChange(form));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (e: IOffer) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(offer: IOffer): void {
    if (!!offer && offer !== this.formControl.value) {
      this.formControl.patchValue(offer);
    }
  }

  public validate(control: AbstractControl): ValidationErrors | null {
    if (this.formControl.invalid) {
      return { invalid: true };
    }
  }

  public trackBy(index: number, offer: IOffer): number {
    return offer.id;
  }
}
