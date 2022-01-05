import { Observable, of, pipe, UnaryFunction } from 'rxjs';
import { catchError, map, startWith } from 'rxjs/operators';

export enum RequestValidityState {
  Load = 'load',
  Success = 'success',
  Error = 'error',
  HasNoUserCode = 'hasNoUserCode',
}

export const toRequestValidityState = (): UnaryFunction<Observable<any>, Observable<RequestValidityState>> =>
  pipe(
    map(() => RequestValidityState.Success),
    catchError(() => of(RequestValidityState.Error)),
    startWith(RequestValidityState.Load),
  );
