import { Injectable } from '@angular/core';
import { IEnvironment } from '@cc/domain/environments';
import { LuApiV3Service } from '@lucca-front/ng/api';

@Injectable()
export class EnvironmentApiSelectService extends LuApiV3Service<IEnvironment> {
  // eslint-disable-next-line @typescript-eslint/naming-convention
  protected _clueFilter(clue: string): string {
    return `subdomain=like,${clue}`;
  }
}
