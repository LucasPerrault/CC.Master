import { Component, OnDestroy, OnInit } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { RightsService } from '@cc/aspects/rights';
import { Observable, Subject } from 'rxjs';
import { filter, takeUntil } from 'rxjs/operators';

import { navigationTabs } from './constants/navigation-tabs.const';
import { INavigationTab } from './models/navigation-tab.interface';
import { NavigationAlertService } from './services/navigation-alert.service';
import { ZendeskHelpService } from './services/zendesk-help.service';

enum NavigationTabState {
  Open = 0,
  Closed = 1,
}

class NavigationTabAlert {
  public key: string;
  public alert$: Observable<string>;
}

@Component({
  selector: 'cc-navigation',
  templateUrl: './navigation.component.html',
  styleUrls: ['./navigation.component.scss'],
})
export class NavigationComponent implements OnInit, OnDestroy {

  public get tabs(): INavigationTab[] {
    return navigationTabs.filter(tab =>
      this.rightsService.hasOperationsByRestrictionMode(tab.restriction.operations, tab.restriction.mode),
    );
  }

  private alerts: { [name: string]: string } = { };
  private states: { [name: string]: NavigationTabState } = { };

  private destroy$: Subject<void> = new Subject<void>();

  constructor(
    private zendeskHelpService: ZendeskHelpService,
    private alertService: NavigationAlertService,
    private rightsService: RightsService,
    private router: Router,
  ) {
    this.zendeskHelpService.setupWebWidget();
  }

  public ngOnInit(): void {
    const navigationEnded$ = this.router.events.pipe(
      takeUntil(this.destroy$),
      filter(e => e instanceof NavigationEnd),
    );

    navigationEnded$.subscribe(async (activatedRoute: NavigationEnd) =>
      await this.openActivatedTabAsync(activatedRoute.url),
    );
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public getIconClass(tab: INavigationTab): string {
    return !!tab.icon ? `icon-${tab.icon}` : '';
  }

  public async toggleAndRefreshAsync(tab: INavigationTab): Promise<void> {
    this.states[tab.name] = this.isOpen(tab) ? NavigationTabState.Closed : NavigationTabState.Open;

    if (this.isOpen(tab)) {
      await this.refreshAlertsAsync(tab);
    }
  }

  public isOpen(tab: INavigationTab): boolean {
    return this.states[tab.name] === NavigationTabState.Open;
  }

  public getAlert(tab: INavigationTab, child: INavigationTab): string {
    const key = this.getTabUrlAsKey(tab, child);
    return this.alerts[key];
  }

  public getChildrenUrl(parent: INavigationTab, child: INavigationTab): string {
    return `${ parent.url }/${ child.url }`;
  }

  public toggleHelp(): void {
    this.zendeskHelpService.toggleWidgetDisplay();
  }

  private async openActivatedTabAsync(url: string): Promise<void> {
    const tabWithChildren = this.tabs.filter(t => !!t.children);

    for (const child of tabWithChildren) {
      if (!this.shouldOpenParentTab(url, child)) {
        continue;
      }

      this.states[child.name] = NavigationTabState.Open;
      await this.refreshAlertsAsync(child);
    }
  }

  private shouldOpenParentTab(url: string, child: INavigationTab): boolean {
    const urlWithNoPrependingSlash = url.startsWith('/') ? url.slice(1) : url;
    const firstSegment = urlWithNoPrependingSlash.split('/')[0];

    return firstSegment.toLowerCase().startsWith(child.url.toLowerCase());
  }

  private async refreshAlertsAsync(tab: INavigationTab, forceUpdate: boolean = false): Promise<void> {
    for (const tabAlert of this.getTabAlerts(tab)) {
      if (!forceUpdate && !!this.alerts[tabAlert.key]) {
        continue;
      }

      this.alerts[tabAlert.key] = await tabAlert.alert$.toPromise();
    }
  }

  private getTabAlerts(tab: INavigationTab): NavigationTabAlert[] {
    let alerts: NavigationTabAlert[] = [];

    if (!!tab.alert) {
      const uniqAlertByTab = { key: this.getTabUrlAsKey(tab), alert$: this.alertService.getAlert$(tab.alert) };
      alerts = [...alerts, uniqAlertByTab];
    }

    if (!!tab.children) {
      alerts = [...alerts, ...this.getChildrenTabAlerts(tab)];
    }

    return alerts;
  }

  private getTabUrlAsKey = (tab: INavigationTab, child: INavigationTab = undefined): string =>
    !!child ? tab.url + '/' + child.url : tab.url;

  private getChildrenTabAlerts(tab: INavigationTab): NavigationTabAlert[] {
    const childrenWithAlert = tab.children.filter(child => !!child.alert);

    return childrenWithAlert.map(child => ({
      key: this.getTabUrlAsKey(tab, child),
      alert$: this.alertService.getAlert$(child.alert),
    }));
  }
}
