import { Observable, pipe, UnaryFunction } from 'rxjs';
import { map } from 'rxjs/operators';

import { SubmissionState } from './submission-state.enum';

export const getInputClass = (state: SubmissionState) => {
  switch (state) {
    case SubmissionState.Success:
      return 'is-valid';
    case SubmissionState.Error:
      return 'is-invalid';
    case SubmissionState.Load:
      return 'is-loading';
    case SubmissionState.Idle:
    default:
      return '';
  }
};

export const toInputClass = (): UnaryFunction<Observable<SubmissionState>, Observable<string>> =>
  pipe(map(state => getInputClass(state)));
