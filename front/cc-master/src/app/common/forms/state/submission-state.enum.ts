import { Observable, of, pipe, timer, UnaryFunction } from 'rxjs';
import { catchError, map, startWith, switchMap, take } from 'rxjs/operators';

export enum SubmissionState {
  Idle = 0,
  Load = 1,
  Error = 2,
  Success = 3,
}

export const toSubmissionState = (): UnaryFunction<Observable<any>, Observable<SubmissionState>> =>
  pipe(
    map(() => SubmissionState.Success),
    catchError(() => of(SubmissionState.Error)),
    switchMap(state => idleAfter$(1000, state)),
    startWith(SubmissionState.Load),
  );

const idleAfter$ = (dueTime: number, state: SubmissionState): Observable<SubmissionState> =>
  timer(dueTime).pipe(
    take(1),
    map(() => SubmissionState.Idle),
    startWith(state),
  );
