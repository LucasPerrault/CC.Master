import { Component, EventEmitter, OnInit } from '@angular/core';
import { SafeResourceUrl } from '@angular/platform-browser';
import { CalendlyEmbedType } from '@cc/common/calendly/model/calendly-format-url.interface';
import { AccountManagersDataService, IAccountManager } from '@cc/common/calendly/services/account-managers/account-managers-data.service';
import { CalendlyService } from '@cc/common/calendly/services/calendly.service';
import { ILuccaModal } from '@cc/common/calendly/services/lucca-modal';
import { BehaviorSubject, Observable, pipe, ReplaySubject, UnaryFunction } from 'rxjs';
import { filter, map, take } from 'rxjs/operators';

@Component({
  selector: 'cc-calendly-modal',
  templateUrl: './calendly-modal.component.html',
  styleUrls: ['./calendly-modal.component.scss'],
})
export class CalendlyModalComponent implements ILuccaModal, OnInit {
  public title = 'Lucca Calendly - Contact us';
  public close$ = new EventEmitter<void>();

  public calendlyUrl$ = new ReplaySubject<SafeResourceUrl>(1);

  public get hasAccountManagers$(): Observable<boolean> {
    return this.accountManagers$.pipe(map(ams => !!ams.length));
  }

  private accountManagers$ = new BehaviorSubject<IAccountManager[]>([]);

  constructor(private dataService: AccountManagersDataService, private calendlyService: CalendlyService) {
    this.title = 'POC Calendly';
  }

  public ngOnInit(): void {
    this.accountManagers$
      .pipe(filter(ams => !!ams.length), this.formatUrl)
      .subscribe(url => this.calendlyUrl$.next(url));

    this.dataService.getAccountManagers$()
      .pipe(take(1))
      .subscribe(ams => this.accountManagers$.next(ams));
  }

  public close(): void {
    this.close$.next();
  }

  private get formatUrl(): UnaryFunction<Observable<IAccountManager[]>, Observable<SafeResourceUrl>> {
    return pipe(
      map(ams => ams[0]),
      map(am => this.calendlyService.getUrl(am, CalendlyEmbedType.PopupWidget)),
    );
  }
}
