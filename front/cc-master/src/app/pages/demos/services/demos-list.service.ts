import { Injectable } from '@angular/core';
import { Observable, ReplaySubject } from 'rxjs';
import { take } from 'rxjs/operators';

import { IDemo } from '../models/demo.interface';
import { DemosDataService } from './demos-data.service';

@Injectable()
export class DemosListService {

  public get resetAll$(): Observable<void> { return this.resetDemos.asObservable(); }
  public get resetOne$(): Observable<IDemo> { return this.resetDemo.asObservable(); }

  private resetDemos = new ReplaySubject<void>(1);
  private resetDemo = new ReplaySubject<IDemo>(1);

  constructor(private dataService: DemosDataService) {}

  public resetAll(): void {
    this.resetDemos.next();
  }

  public resetOne(demoId: number): void {
    this.dataService.getDemo$(demoId)
      .pipe(take(1))
      .subscribe(demo => this.resetDemo.next(demo));
  }
}
