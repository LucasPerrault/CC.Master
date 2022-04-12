import { ChangeDetectionStrategy, Component, forwardRef, OnDestroy, OnInit } from '@angular/core';
import {
  AbstractControl,
  ControlValueAccessor,
  FormControl,
  NG_VALIDATORS,
  NG_VALUE_ACCESSOR,
  ValidationErrors,
  Validator,
  Validators,
} from '@angular/forms';
import { getInputClass, SubmissionState, toInputClass } from '@cc/common/forms';
import { BehaviorSubject, Observable, of, pipe, ReplaySubject, Subject, UnaryFunction } from 'rxjs';
import { catchError, filter, map, take, takeUntil } from 'rxjs/operators';

import { demoDomain } from '../../../models/demo.interface';
import { SubdomainAvailabilityDataService } from './subdomain-availability-data.service';
import { getSubdomainAvailabilityStatus, SubdomainAvailabilityStatus } from './subdomain-availability-status.enum';

@Component({
  selector: 'cc-demo-client-subdomain-input',
  templateUrl: './demo-client-subdomain-input.component.html',
  styleUrls: ['./demo-client-subdomain-input.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => DemoClientSubdomainInputComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: DemoClientSubdomainInputComponent,
    },
  ],
})
export class DemoClientSubdomainInputComponent implements ControlValueAccessor, Validator, OnInit, OnDestroy {

  public subdomain: FormControl = new FormControl('', Validators.required);
  public domain = demoDomain;

  public hasSubdomainAvailabilityError$ = new BehaviorSubject<boolean>(false);
  public subdomainAvailabilityClass$ = new ReplaySubject<string>(1);
  public subdomainAvailabilityMessage$ = new ReplaySubject<string>(1);
  private subdomainAvailabilityStatus$ = new ReplaySubject<SubdomainAvailabilityStatus>(1);
  private previousCheckedSubdomain$ = new BehaviorSubject<string>('');

  private destroy$: Subject<void> = new Subject();

  constructor(private subdomainAvailabilityService: SubdomainAvailabilityDataService) {}

  public ngOnInit(): void {
    this.subdomainAvailabilityStatus$
      .pipe(this.toAvailabilityState, toInputClass())
      .subscribe(c => this.subdomainAvailabilityClass$.next(c));

    this.subdomainAvailabilityStatus$
      .pipe(takeUntil(this.destroy$), this.toStatusMessage)
      .subscribe(m => this.subdomainAvailabilityMessage$.next(m));

    this.subdomainAvailabilityStatus$
      .pipe(takeUntil(this.destroy$), map(status => status !== SubdomainAvailabilityStatus.Ok))
      .subscribe(h => {
        this.hasSubdomainAvailabilityError$.next(h);
        this.onValidatorChange();
      });

    this.subdomainAvailabilityStatus$
      .pipe(takeUntil(this.destroy$), filter(status => status === SubdomainAvailabilityStatus.Ok))
      .subscribe(() => this.onChange(this.subdomain.value));

    this.subdomain.valueChanges
      .pipe(takeUntil(this.destroy$), map(subdomain => !subdomain))
      .subscribe(() => this.subdomainAvailabilityStatus$.next(null));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (subdomain: string) => void = () => {};
  public onTouch: () => void = () => {};
  public onValidatorChange: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public registerOnValidatorChange(fn: () => void) {
    this.onValidatorChange = fn;
  }

  public writeValue(subdomain: string): void {
    if (!!subdomain && subdomain !== this.subdomain.value) {
      this.subdomain.patchValue(subdomain);
    }
  }

  public validate(control: AbstractControl): ValidationErrors | null {
    if (this.hasSubdomainAvailabilityError$.value || this.subdomain.invalid) {
      return { invalid: true };
    }
  }

  public checkSubdomainAvailability(): void {
    const previousSubdomain = this.previousCheckedSubdomain$.value;
    this.previousCheckedSubdomain$.next(this.subdomain.value);

    if (!this.subdomain.value || this.subdomain.value === previousSubdomain) {
      return;
    }

    this.subdomainAvailabilityClass$.next(getInputClass(SubmissionState.Load));

    this.subdomainAvailabilityService.getStatus$(this.subdomain.value)
      .pipe(take(1))
      .subscribe(status => this.subdomainAvailabilityStatus$.next(status));
  }

  private get toAvailabilityState(): UnaryFunction<Observable<SubdomainAvailabilityStatus>, Observable<SubmissionState>> {
    return pipe(
      map(status => this.getSubmissionState(status)),
      catchError(() => of(SubmissionState.Error)),
    );
  }

  private get toStatusMessage(): UnaryFunction<Observable<SubdomainAvailabilityStatus>, Observable<string>> {
    return pipe(map(status => getSubdomainAvailabilityStatus(status)?.message));
  }

  private getSubmissionState(status: SubdomainAvailabilityStatus): SubmissionState {
    if (status === null) {
      return SubmissionState.Idle;
    }

    return status === SubdomainAvailabilityStatus.Ok ? SubmissionState.Success : SubmissionState.Error;
  }

}
