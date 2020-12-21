import {Inject, Injectable} from '@angular/core';
import {IPrincipal, PRINCIPAL} from '@cc/aspects/principal';
import {Operation} from '@cc/aspects/rights';
import {IPermission} from '@cc/aspects/rights';

@Injectable()
export class RightsService {
  constructor(@Inject(PRINCIPAL) public principal: IPrincipal) {}

  public hasOperation(operation: Operation): boolean {
    return this.hasOperations([operation]);
  }

  public hasOperations(operations: Operation[]): boolean {
    const permissions = this.getPermissionsByOperations(operations);
    return !!permissions.length;
  }

  private getPermissionsByOperations(operations: Operation[]): IPermission[] {
    return this.principal.permissions.filter(p => operations.includes(p.operation.id));
  }
}
