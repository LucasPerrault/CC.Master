import { ChangeDetectionStrategy, Component, EventEmitter, OnInit, Output } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { SubmissionState } from '@cc/common/forms';
import { BehaviorSubject, Observable, of, ReplaySubject } from 'rxjs';
import { catchError, map, startWith, take, tap } from 'rxjs/operators';

import { IUploadedOffer } from '../uploaded-offer.interface';
import { OfferUploadService } from './offer-upload.service';

@Component({
  selector: 'cc-offer-upload',
  templateUrl: './offer-upload.component.html',
  styleUrls: ['./offer-upload.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class OfferUploadComponent implements OnInit {
  @Output() public upload: EventEmitter<IUploadedOffer[]> = new EventEmitter<IUploadedOffer[]>();
  public fileName$: ReplaySubject<string> = new ReplaySubject<string>(1);

  public get isLoading$(): Observable<boolean> {
    return this.state$.pipe(map(state => state === SubmissionState.Load));
  }

  public get isError$(): Observable<boolean> {
    return this.state$.pipe(map(state => state === SubmissionState.Error));
  }

  public get fileClass$(): Observable<string> {
    return this.state$.pipe(map(state => this.getFileClass(state)));
  }

  public get title$(): Observable<string> {
    return this.state$.pipe(map(state => this.getTitle(state)));
  }

  public get buttonTitle$(): Observable<string> {
    return this.state$.pipe(map(state => this.getButtonTitle(state)));
  }

  private state$: BehaviorSubject<SubmissionState> = new BehaviorSubject<SubmissionState>(SubmissionState.Idle);

  constructor(private uploadService: OfferUploadService, private translatePipe: TranslatePipe) { }

  ngOnInit(): void {
  }

  public fileChange(event: any): void {
    if (!event.target?.files?.length) {
      return;
    }

    const file = event.target.files[0];

    this.uploadService.upload$(file)
      .pipe(
        take(1),
        tap(offers => this.upload.emit(offers)),
        map(() => SubmissionState.Success),
        catchError(() => {
          this.upload.emit([]);
          return of(SubmissionState.Error);
        }),
        startWith(SubmissionState.Load),
      )
      .subscribe(state => {
        this.state$.next(state);
        this.fileName$.next(file.name);
      });
  }

  private getFileClass(state: SubmissionState): string {
    switch (state) {
      case SubmissionState.Idle:
        return 'is-droppable';
      case SubmissionState.Success:
        return 'is-uploaded';
      case SubmissionState.Error:
      case SubmissionState.Load:
      default:
        return '';
    }
  }

  private getTitle(state: SubmissionState): string {
    switch (state) {
      case SubmissionState.Idle:
        return this.translatePipe.transform('offers_import_file_title');
      case SubmissionState.Success:
        return this.translatePipe.transform('offers_import_file_title_success');
      case SubmissionState.Error:
        return this.translatePipe.transform('offers_import_file_title_error');
      case SubmissionState.Load:
      default:
        return '';
    }
  }

  private getButtonTitle(state: SubmissionState): string {
    switch (state) {
      case SubmissionState.Idle:
        return this.translatePipe.transform('offers_import_file_button');
      case SubmissionState.Success:
      case SubmissionState.Error:
        return this.translatePipe.transform('offers_import_file_button_another');
      case SubmissionState.Load:
      default:
        return '';
    }
  }

}
