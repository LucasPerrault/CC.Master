import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TranslatePipe } from '@cc/aspects/translate';
import { Observable, ReplaySubject } from 'rxjs';
import { finalize, map, startWith, take } from 'rxjs/operators';

import { ContractLogType, contractLogTypes } from './constants/contract-log-type.enum';
import { IContractLog } from './models/contract-log.interface';
import { ContractLogsService } from './services/contract-logs.service';

@Component({
  selector: 'cc-history-tab',
  templateUrl: './history-tab.component.html',
  styleUrls: ['./history-tab.component.scss'],
})
export class HistoryTabComponent implements OnInit {

  public contractLogs$: ReplaySubject<IContractLog[]> = new ReplaySubject<IContractLog[]>(1);
  public isLoading$: ReplaySubject<boolean> = new ReplaySubject<boolean>(1);

  public get isEmpty$(): Observable<boolean> {
    return this.contractLogs$.pipe(map(contractLogs => !contractLogs.length));
  }

  private get contractId(): number {
    return parseInt(this.activatedRoute.parent.snapshot.paramMap.get('id'), 10);
  }

  constructor(
    private activatedRoute: ActivatedRoute,
    private contractLogsService: ContractLogsService,
    private translatePipe: TranslatePipe,
  ) { }

  public ngOnInit(): void {
    this.isLoading$.next(true);

    this.contractLogsService.getContractLogs$(this.contractId)
      .pipe(take(1), finalize(() => this.isLoading$.next(false)), startWith([]))
      .subscribe(logs => this.contractLogs$.next(logs));
  }

  public getContractLogType(contractLogType: ContractLogType): string {
    const contractLog = contractLogTypes.find(l => l.id === contractLogType);
    return !!contractLog ? this.translatePipe.transform(contractLog.name) : '';
  }
}
