import { SubmissionState } from './submission-state.enum';

export const getButtonState = (state: SubmissionState) => {
  switch (state) {
    case SubmissionState.Success:
      return 'is-success';
    case SubmissionState.Error:
      return 'is-error';
    case SubmissionState.Load:
      return 'is-loading';
    case SubmissionState.Idle:
    default:
      return '';
  }
};
