import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { HttpClient } from '@angular/common/http';

export interface IAccountManager {
  id: number;
  name: string;
  email: string;
  companyId: string;
  picture: IAccountManagerPicture;
}

export interface IAccountManagerPicture {
  href: string;
}

@Injectable()
export class AccountManagersDataService {
  constructor(private httpClient: HttpClient) {}

  public getAccountManagers$(): Observable<IAccountManager[]> {
    // How to fetch ams from any apps ?
    // lucca-banners did it
    // To communicate with EC it needs to be installed ?
    // We need to be auth ? lucca-principal ?
    return of([
      {
        email: 'hcerutti@lucca.fr',
      } as IAccountManager,
    ]);
  }

  private get$(): Observable<IAccountManager[]> {
    const url = 'admin/account-and-billing/services/communication/account-managers';
    return this.httpClient.get<IAccountManager[]>(url);
  }
}
