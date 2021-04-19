import { Component, OnDestroy, OnInit } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { Subject } from 'rxjs';
import { filter, takeUntil } from 'rxjs/operators';

import { INavigationTab } from './models/navigation-tab.interface';
import { NavigationTabsService } from './services/navigation-tabs.service';

@Component({
  selector: 'cc-navigation',
  templateUrl: './navigation.component.html',
  styleUrls: ['./navigation.component.scss'],
})
export class NavigationComponent implements OnInit, OnDestroy {

  public get tabs(): INavigationTab[] {
    return this.navigationService.tabs;
  }

  private destroy$: Subject<void> = new Subject<void>();

  constructor(
    private navigationService: NavigationTabsService,
    private router: Router,
  ) { }

  public ngOnInit(): void {
    const navigationEnded$ = this.router.events.pipe(
      takeUntil(this.destroy$),
      filter(e => e instanceof NavigationEnd),
    );

    navigationEnded$.subscribe(async (activatedRoute: NavigationEnd) =>
      await this.navigationService.openActivatedTabAsync(activatedRoute.url),
    );
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public getIconClass(tab: INavigationTab): string {
    return !!tab.icon ? `icon-${tab.icon}` : '';
  }

  public async toggleAsync(tab: INavigationTab): Promise<void> {
    await this.navigationService.toggleState(tab);

    if (this.isOpen(tab)) {
      await this.navigationService.refreshAlertsAsync(tab);
    }
  }

  public isOpen(tab: INavigationTab): boolean {
    return this.navigationService.isOpen(tab);
  }

  public getAlert(tab: INavigationTab, child: INavigationTab): string {
    return this.navigationService.getAlert(tab, child);
  }
}
