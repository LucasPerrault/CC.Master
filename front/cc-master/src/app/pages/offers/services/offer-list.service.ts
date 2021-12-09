import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';

@Injectable()
export class OfferListService {
  public get refresh$(): Observable<void> {
    return this.onRefresh$.asObservable();
  }

  private onRefresh$: Subject<void> = new Subject<void>();

  public refresh(): void {
    this.onRefresh$.next();
  }
}
