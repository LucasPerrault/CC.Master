import { Injectable } from '@angular/core';
import {
  AccountManagersDataService,
  IAccountManager,
} from '@cc/common/calendly/services/account-managers/account-managers-data.service';
import { BehaviorSubject, Observable } from 'rxjs';
import { take } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class AccountManagersStoreService {
  public get managers$(): Observable<IAccountManager[]> {
    return this.managers.asObservable();
  }

  private managers = new BehaviorSubject<IAccountManager[]>([]);

  constructor(private dataService: AccountManagersDataService) {
    this.init();
  }

  private init(): void {
    this.reset();
  }

  private reset(): void {
    this.dataService.getAccountManagers$()
      .pipe(take(1))
      .subscribe(ams => this.managers.next(ams));
  }
}
