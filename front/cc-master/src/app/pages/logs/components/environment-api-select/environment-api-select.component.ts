import { Component, EventEmitter, Output } from '@angular/core';
import { ALuApiService } from '@lucca-front/ng/api';

import { IEnvironment } from '../../models';
import { EnvironmentApiSelectService } from './environment-api-select.service';

@Component({
  selector: 'cc-environment-api-select',
  templateUrl: './environment-api-select.component.html',
  providers: [{
    provide: ALuApiService, useClass: EnvironmentApiSelectService,
  }],
})
export class EnvironmentApiSelectComponent {
  @Output() public environmentIdsToString: EventEmitter<string> = new EventEmitter<string>();
  public environmentIds: number[];
  public apiUrl = 'api/v3/environments';
  public apiFields = 'subdomain';
  public apiOrderBy = 'subdomain,asc';

  public trackBy(index: number, domain: IEnvironment): string {
    return domain.subDomain;
  }

  public updateEnvironmentIdsSelected(environmentIds: number[]): void {
    if (!environmentIds) {
      this.environmentIdsToString.emit('');
      return;
    }

    const environmentIdsToString = environmentIds.join(',');
    this.environmentIdsToString.emit(environmentIdsToString);
  }
}
