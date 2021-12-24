import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NoNavComponent } from '@cc/common/routing';

enum DistributorDomainRoutingParams {
  Domain = 'domain',
  Registered = 'registered',
}

@Component({
  styleUrls: ['./distributor-domain.component.scss'],
  selector: 'cc-distributor-domain',
  templateUrl: './distributor-domain.component.html',
})
export class DistributorDomainComponent extends NoNavComponent implements OnInit {
  public domain: string;
  public registered: string;

  public get userEmail(): string {
    return `<b>xxxx@${this.domain}</b>`;
  }

  public get registeredDomains(): string {
    return `<b>${this.registered}</b>`;
  }

  constructor(private activatedRoute: ActivatedRoute) {
    super();
  }

  public ngOnInit(): void {
    const queryParams = this.activatedRoute.snapshot.queryParamMap;
    this.domain = queryParams.get(DistributorDomainRoutingParams.Domain);
    this.registered = queryParams.get(DistributorDomainRoutingParams.Registered);
  }
}
