import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';

@Injectable()
export class ContractsManageModalService {
  private onClose: Subject<void> = new Subject<void>();
  public get onClose$(): Observable<void> {
    return this.onClose.asObservable();
  }

  private onDismiss: Subject<void> = new Subject<void>();
  public get onDismiss$(): Observable<void> {
    return this.onDismiss.asObservable();
  }

  private onRefresh: Subject<void> = new Subject<void>();
  public get onRefresh$(): Observable<void> {
    return this.onRefresh.asObservable();
  }

  public close(): void {
    this.onClose.next();
  }

  public dismiss(): void {
    this.onDismiss.next();
  }

  public refresh(): void {
    this.onRefresh.next();
  }
}
