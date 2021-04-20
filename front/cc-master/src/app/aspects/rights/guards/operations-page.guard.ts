import { Inject, Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { forbiddenUrl } from '@cc/common/error-redirections';
import { INavigationTab } from '@cc/common/navigation';
import { NAVIGATION_TABS } from '@cc/common/navigation/navigation-tabs.token';

import { RightsService } from '../rights.service';

@Injectable()
export class OperationsPageGuard implements CanActivate {
  constructor(
    @Inject(NAVIGATION_TABS) private navigationTabs: INavigationTab[],
    private router: Router,
    private rightsService: RightsService,
  ) { }

  canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree {
    const activatedTab = this.navigationTabs.find(tab => tab.url === state.url.slice(1));

    const hasOperations = !!activatedTab && this.rightsService.hasOperationsByRestrictionMode(
      activatedTab.restriction.operations,
      activatedTab.restriction.mode,
    );

    return hasOperations || this.router.parseUrl(forbiddenUrl);
  }
}
