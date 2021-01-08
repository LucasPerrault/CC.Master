import { Component, EventEmitter, Output } from '@angular/core';

import { environmentDomains } from '../../enums';
import { IEnvironmentDomain } from '../../models';

@Component({
  selector: 'cc-environment-domain-select',
  templateUrl: './environment-domain-select.component.html',
})
export class EnvironmentDomainSelectComponent {
  @Output() public domainIdsToString: EventEmitter<string> = new EventEmitter<string>();

  public environmentDomainIds: number[];

  public searchFn(domain: IEnvironmentDomain, clue: string): boolean {
    return domain.name.toLowerCase().includes(clue.toLowerCase());
  }

  public trackBy(index: number, domain: IEnvironmentDomain): string {
    return domain.name;
  }

  public updateEnvironmentDomainIdsSelected(domainIds: number[]) {
    if (!domainIds) {
      return;
    }
    const domainIdsToString = domainIds.join(',');
    this.domainIdsToString.emit(domainIdsToString);
  }

  public get environmentDomains(): IEnvironmentDomain[] {
    return environmentDomains;
  }
}
