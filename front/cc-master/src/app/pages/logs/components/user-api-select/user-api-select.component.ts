import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { IPrincipal } from '@cc/aspects/principal';
import { ALuApiService } from '@lucca-front/ng/api';

import { UserApiSelectService } from './user-api-select.service';

@Component({
  selector: 'cc-user-api-select',
  templateUrl: './user-api-select.component.html',
  providers: [{
    provide: ALuApiService, useClass: UserApiSelectService,
  }],
})
export class UserApiSelectComponent implements OnInit {
  @Output() public userIdsToString: EventEmitter<string> = new EventEmitter<string>();
  public userIds: number[];
  private routerParamKey = 'userIds';

  constructor(private router: Router, private activatedRoute: ActivatedRoute) {
  }

  public ngOnInit() {
    const routerParamValue = this.activatedRoute.snapshot.queryParamMap.get(this.routerParamKey);
    if (!routerParamValue) {
      return;
    }

    this.userIdsToString.emit(routerParamValue);
    this.userIds = routerParamValue.split(',').map(i => parseInt(i, 10));
  }

  public trackBy(index: number, user: IPrincipal): string {
    return user.name;
  }

  public async updateUserIdsSelectedAsync(userIds: number[]): Promise<void> {
    const userIdsToString = !!userIds ? userIds.join(',') : '';
    this.userIdsToString.emit(userIdsToString);

    await this.updateRouterAsync(userIdsToString);
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
