import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
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
export class EnvironmentApiSelectComponent implements OnInit {
  @Output() public environmentIdsToString: EventEmitter<string> = new EventEmitter<string>();
  public environmentSubDomains: string[];
  public apiUrl = 'api/v3/environments';
  public apiFields = 'id,subdomain';
  public apiOrderBy = 'subdomain,asc';
  private routerParamKey = 'subDomains';

  constructor(private router: Router, private activatedRoute: ActivatedRoute) {
  }

  public ngOnInit(): void {
    const routerParamValue = this.activatedRoute.snapshot.queryParamMap.get(this.routerParamKey);
    if (!routerParamValue) {
      return;
    }

    this.environmentIdsToString.emit(routerParamValue);
    this.environmentSubDomains = routerParamValue.split(',');
  }

  public trackBy(index: number, domain: IEnvironment): string {
    return domain.subDomain;
  }

  public async updateEnvironmentIdsSelectedAsync(environmentSubDomains: string[]): Promise<void> {
    const environmentSubDomainsToQuery = !!environmentSubDomains ? environmentSubDomains.join(',') : '';
    this.environmentIdsToString.emit(environmentSubDomainsToQuery);

    await this.updateRouterAsync(environmentSubDomainsToQuery);
  }

  private async updateRouterAsync(value: string): Promise<void> {
    const queryParams = { [this.routerParamKey]: !!value ? value : null };

    await this.router.navigate([], {
      relativeTo: this.activatedRoute,
      queryParams,
      queryParamsHandling: 'merge',
    });
  }
}
