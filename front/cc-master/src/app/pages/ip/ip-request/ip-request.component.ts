import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NoNavComponent } from '@cc/common/routing';

enum IpRequestRoutingParams {
  Ip = 'ip',
}

@Component({
  selector: 'cc-ip-request',
  templateUrl: './ip-request.component.html',
  styleUrls: ['./ip-request.component.scss'],
})
export class IpRequestComponent extends NoNavComponent implements OnInit {

  public userIp: string;

  constructor(private activatedRoute: ActivatedRoute) {
    super();
  }

  public ngOnInit(): void {
    const queryParams = this.activatedRoute.snapshot.queryParamMap;
    this.userIp = queryParams.get(IpRequestRoutingParams.Ip);
  }

}
