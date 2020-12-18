import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree} from '@angular/router';
import {Operation} from '@cc/aspects/rights';
import { RightsService } from '@cc/aspects/rights';
import {forbiddenUrl} from '@cc/common/errors';

@Injectable()
export class OperationsGuard implements CanActivate {

    public constructor(private rightsService: RightsService, private router: Router) { }

    canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree {
        const operations = next.data.operations as Operation[];

        return this.hasOperations(operations) || this.router.parseUrl(forbiddenUrl);
    }

    public hasOperations(operations: Operation[]): boolean {
        return this.rightsService.hasOperations(operations);
    }
}
