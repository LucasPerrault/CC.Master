import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { environmentDomains } from '../../enums';
import { IEnvironmentDomain } from '../../models';

@Component({
  selector: 'cc-environment-domain-select',
  templateUrl: './environment-domain-select.component.html',
})
export class EnvironmentDomainSelectComponent implements OnInit {
  @Output() public domainIdsToString: EventEmitter<string> = new EventEmitter<string>();

  public environmentDomainIds: number[];
  private routerParamKey = 'domainIds';

  constructor(private router: Router, private activatedRoute: ActivatedRoute ) {
  }

  ngOnInit() {
    const routerParamValue = this.activatedRoute.snapshot.queryParamMap.get(this.routerParamKey);
    if (!routerParamValue) {
      return;
    }

    this.domainIdsToString.emit(routerParamValue);
    this.environmentDomainIds = routerParamValue.split(',').map(i => parseInt(i, 10));
  }

  public searchFn(domain: IEnvironmentDomain, clue: string): boolean {
    return domain.name.toLowerCase().includes(clue.toLowerCase());
  }

  public trackBy(index: number, domain: IEnvironmentDomain): string {
    return domain.name;
  }

  public async updateEnvironmentDomainIdsSelectedAsync(domainIds: number[]) {
    const domainIdsToString = !!domainIds ? domainIds.join(',') : '';
    this.domainIdsToString.emit(domainIdsToString);

    await this.updateRouterAsync(domainIdsToString);
  }

  private async updateRouterAsync(value: string): Promise<void> {
    const queryParams = { [this.routerParamKey]: !!value ? value : null };

    await this.router.navigate([], {
      relativeTo: this.activatedRoute,
      queryParams,
      queryParamsHandling: 'merge',
    });
  }

  public get environmentDomains(): IEnvironmentDomain[] {
    return environmentDomains;
  }
}
