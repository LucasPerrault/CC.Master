import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { INavigationTab } from '@cc/common/navigation';
import { NoNavPath } from '@cc/common/routing';

import { RightsService } from '../rights.service';

@Injectable()
export class OperationsPageGuard implements CanActivate {
  constructor(
    private router: Router,
    private rightsService: RightsService,
  ) { }

  canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree {
    const navigationTabs = next.data.tabs as INavigationTab[] | [];

    const activatedTabPath = next.firstChild?.routeConfig.path;
    const activatedTab = navigationTabs.find((tab: INavigationTab) => tab.url === activatedTabPath);

    if (!activatedTab) {
      return this.router.parseUrl(NoNavPath.NotFound);
    }

    const hasOperations = !!activatedTab && this.rightsService.hasOperationsByRestrictionMode(
      activatedTab.restriction.operations,
      activatedTab.restriction.mode,
    );

    return hasOperations || this.router.parseUrl(NoNavPath.Forbidden);
  }
}
