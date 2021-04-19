import { Inject, Injectable } from '@angular/core';
import { IPrincipal, PRINCIPAL } from '@cc/aspects/principal';
import { RightsService } from '@cc/aspects/rights';
import { Observable } from 'rxjs';

import { INavigationTab } from '../models/navigation-tab.interface';
import { NAVIGATION_TABS } from '../navigation-tabs.token';

enum NavigationTabState {
  Open = 0,
  Closed = 1,
}

class NavigationTabAlert {
  public key: string;
  public alert$: Observable<string>;
}

@Injectable()
export class NavigationTabsService {
  private alerts: { [name: string]: string } = { };
  private states: { [name: string]: NavigationTabState } = { };

  public get tabs(): INavigationTab[] {
    return this.navigationTabs.filter(tab =>
      this.rightsService.hasOperationsByRestrictionMode(tab.restriction.operations, tab.restriction.mode),
    );
  }

  constructor(
    @Inject(PRINCIPAL) private principal: IPrincipal,
    @Inject(NAVIGATION_TABS) private navigationTabs: INavigationTab[],
    private rightsService: RightsService,
  ) {}

  public toggleState(tab: INavigationTab): void {
    this.states[tab.name] = this.isOpen(tab)
      ? NavigationTabState.Closed
      : NavigationTabState.Open;
  }

  public isOpen(tab: INavigationTab): boolean {
    return this.states[tab.name] === NavigationTabState.Open;
  }

  public async openActivatedTabAsync(url: string): Promise<void> {
    const tabWithChildren = this.tabs.filter(t => !!t.children);

    for (const child of tabWithChildren) {
      if (!this.shouldOpenParentTab(url, child)) {
        continue;
      }

      this.states[child.name] = NavigationTabState.Open;
      await this.refreshAlertsAsync(child);
    }
  }

  public async refreshAlertsAsync(tab: INavigationTab, forceUpdate: boolean = false): Promise<void> {
    for (const tabAlert of this.getTabAlerts(tab)) {
      if (!forceUpdate && !!this.alerts[tabAlert.key]) {
        continue;
      }

      this.alerts[tabAlert.key] = await tabAlert.alert$.toPromise();
    }
  }

  public getAlert(tab: INavigationTab, child: INavigationTab): string {
    const key = this.getTabUrlAsKey(tab, child);
    return this.alerts[key];
  }

  private shouldOpenParentTab(url: string, child: INavigationTab): boolean {
    const urlWithNoPrependingSlash = url.startsWith('/') ? url.slice(1) : url;
    const firstSegment = urlWithNoPrependingSlash.split('/')[0];

    return firstSegment.toLowerCase().startsWith(child.url.toLowerCase());
  }

  private getTabAlerts(tab: INavigationTab): NavigationTabAlert[] {
    let alerts: NavigationTabAlert[] = [];

    if (!!tab.alert$) {
      const uniqAlertByTab = { key: this.getTabUrlAsKey(tab), alert$: tab.alert$ };
      alerts = [...alerts, uniqAlertByTab];
    }

    if (!!tab.children) {
      alerts = [...alerts, ...this.getChildrenTabAlerts(tab)];
    }

    return alerts;
  }

  private getChildrenTabAlerts(tab: INavigationTab): NavigationTabAlert[] {
    const childrenWithAlert = tab.children.filter(child => !!child.alert$);

    return childrenWithAlert.map(child => ({
      key: this.getTabUrlAsKey(tab, child),
      alert$: child.alert$,
    }));
  }

  private getTabUrlAsKey = (tab: INavigationTab, child: INavigationTab = undefined): string =>
    !!child ? tab.url + '/' + child.url : tab.url;
}
