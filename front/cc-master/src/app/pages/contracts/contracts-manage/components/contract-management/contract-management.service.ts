import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';

@Injectable()
export class ContractManagementService {
  private onClose: Subject<void> = new Subject<void>();
  public get onClose$(): Observable<void> {
    return this.onClose.asObservable();
  }

  private onRefresh: Subject<void> = new Subject<void>();
  public get onRefresh$(): Observable<void> {
    return this.onRefresh.asObservable();
  }

  public close(): void {
    this.onClose.next();
  }

  public refresh(): void {
    this.onRefresh.next();
  }
}
