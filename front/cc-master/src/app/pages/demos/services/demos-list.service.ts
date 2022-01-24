import { Injectable } from '@angular/core';
import { Observable, ReplaySubject } from 'rxjs';

@Injectable()
export class DemosListService {

  public get resetAll$(): Observable<void> { return this.reset.asObservable(); }
  private reset = new ReplaySubject<void>(1);

  public resetAll(): void {
    this.reset.next();
  }
}
