import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Operation } from '@cc/aspects/rights/enums/operation.enum';
import { OperationRestrictionMode } from '@cc/aspects/rights/enums/operation-restriction-mode.enum';
import { RightsService } from '@cc/aspects/rights/rights.service';
import { NoNavPath } from '@cc/common/routing';

@Injectable()
export class OperationsGuard implements CanActivate {

  public constructor(private rightsService: RightsService, private router: Router) {
  }

  canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree {
    const operations = next.data.operations as Operation[] | [];
    const mode = next?.data?.mode as OperationRestrictionMode;

    return this.hasOperations(operations, mode) || this.router.parseUrl(NoNavPath.Forbidden);
  }

  public hasOperations(operations: Operation[], mode: OperationRestrictionMode): boolean {
    return this.rightsService.hasOperationsByRestrictionMode(operations, mode);
  }
}
