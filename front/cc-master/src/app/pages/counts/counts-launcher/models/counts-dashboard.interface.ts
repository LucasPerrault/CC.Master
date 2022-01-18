import { SubmissionState } from '@cc/common/forms';
import { IContract } from '@cc/domain/billing/contracts';
import { ReplaySubject } from 'rxjs';

export class CountsDashboard {
  countPeriod: Date;

  numberOfContracts$ = new ReplaySubject<number>(1);
  isNumberOfContractsLoading$ = new ReplaySubject<boolean>(1);

  numberOfRealCounts$ = new ReplaySubject<number>(1);
  isNumberOfRealCountsLoading$ = new ReplaySubject<boolean>(1);

  contractsWithoutCounts$ = new ReplaySubject<IContract[]>(1);
  numberOfContractsWithoutCount$ = new ReplaySubject<number>(1);
  isContractsWithoutCountLoading$ = new ReplaySubject<boolean>(1);

  contractsWithDraftCount$ = new ReplaySubject<IContract[]>(1);
  numberOfContractsWithDraftCount$ = new ReplaySubject<number>(1);
  isContractsWithDraftCountLoading$ = new ReplaySubject<boolean>(1);
  withDraftCountRedirectionState$ = new ReplaySubject<SubmissionState>(1);

  contractsWithCountWithoutAccountingEntry$ = new ReplaySubject<IContract[]>(1);
  numberOfContractsWithCountWithoutAccountingEntry$ = new ReplaySubject<number>(1);
  isContractsWithCountWithoutAccountingEntryLoading$ = new ReplaySubject<boolean>(1);
  withCountWithoutAccountingEntryRedirectionState$ = new ReplaySubject<SubmissionState>(1);
}
