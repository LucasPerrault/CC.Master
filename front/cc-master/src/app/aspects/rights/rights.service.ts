import { Inject, Injectable } from '@angular/core';
import { IPrincipal, PRINCIPAL } from '@cc/aspects/principal';
import { Operation } from '@cc/aspects/rights/enums/operation.enum';
import { IPermission } from '@cc/aspects/rights/permission.interface';

import { OperationRestrictionMode } from './enums/operation-restriction-mode.enum';

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

  public hasOperationsByRestrictionMode(operations: Operation[], mode: OperationRestrictionMode): boolean {
    if (!operations || !operations.length) {
      return true;
    }

    switch (mode) {
      case OperationRestrictionMode.Some:
        return operations.some(o => this.hasOperation(o));
      case OperationRestrictionMode.All:
        return operations.every(o => this.hasOperation(o));
      default :
        return this.hasOperations(operations);
    }
  }

  private getPermissionsByOperations(operations: Operation[]): IPermission[] {
    return this.principal.permissions.filter(p => operations.includes(p.operation.id));
  }
}
