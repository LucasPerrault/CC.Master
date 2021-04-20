import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { NoNavPath } from '@cc/common/routing';

import { RightsService } from '../rights.service';

@Injectable()
export class AnyOperationsGuard implements CanActivate {

  public constructor(private rightsService: RightsService, private router: Router) {
  }

  canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree {
    return this.rightsService.hasAnyOperation() || this.router.parseUrl(NoNavPath.Forbidden);
  }
}
