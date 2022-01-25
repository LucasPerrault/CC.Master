import { Injectable } from '@angular/core';
import { IInstanceDuplication, InstanceDuplicationProgress, InstancesDuplicationsDataService } from '@cc/domain/instances';
import { Observable, timer } from 'rxjs';
import { expand, switchMap, take, timeout } from 'rxjs/operators';

@Injectable()
export class InstanceDuplicationsService {
  private readonly pingRepeatDelay = 10000;
  private readonly pingTimeout = 600000;

  constructor(private dataService: InstancesDuplicationsDataService) {}

  public listenDuplication$(duplicationId: string): Observable<IInstanceDuplication> {
    const getDuplication$ = this.getDuplication$(duplicationId);

    return getDuplication$.pipe(
      expand(() => timer(this.pingRepeatDelay).pipe(switchMap(() => getDuplication$))),
      timeout(this.pingTimeout),
    );
  }

  private getDuplication$(duplicationId: string): Observable<IInstanceDuplication> {
    return this.dataService.getDuplication$(duplicationId).pipe(take(1));
  }

  private isInProgress(duplication: IInstanceDuplication): boolean {
    return !duplication?.progress
      || duplication.progress === InstanceDuplicationProgress.Pending
      || duplication.progress === InstanceDuplicationProgress.Running;
  }
}
